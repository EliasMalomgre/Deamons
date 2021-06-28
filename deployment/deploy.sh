#!/bin/bash
#============================================================
#
# FILE: deploy.sh
#
# DESCRIPTION: This script is used to deploy the integration project using google cloud
#
# USAGE: ./deploy.sh test [ARGUMENT] | prod  [ARGUMENT] | deleteall
#
# ARGUMENTS
#        -d --delete: delete everything related to the environment including backups
#        -b --backup: makes a backup of the database of at the moment it is called
#        -r --recover: restores the database with a chosen databasebackup
#        --help: show the help screen
#	 (prod only)
#        -s --size n: deploy n amount of application instances. Default 2
#
# PREREQUISITES
#	 MySQL, gcloud and gsutil need to be installed.
#	 Gitlab needs to be configured to allow ssh cloning
#	 gcloud and gsutil need to have a default region/zone
#	 a directory with sslprivatekey and sslcertification (default path ./rsakeys/)
#	 change gcloudacc variable into own gcloud project id
#
# AUTHOR: Daemons
#
# VERSION: 1.0
#
# EXTRA INFORMATION
# 	gcloud sql instances cannot have the same name even after deletion for at least one week
#
#
#===========================================================

readonly iteration="meervar11"
#########test only variables###########
readonly testGate80="test-allow-80-$iteration"
readonly testDB="test-sql-$iteration"
readonly testVM="test-ubuntu-$iteration"
readonly testBucket="gs://daemontestbucket-$iteration"
#########production only variables#################
readonly prodForwardingRule="prod-forwardingrule-$iteration"
readonly prodHttpProxy="prod-httpproxy-$iteration"
readonly prodWebMap="prod-webmap-$iteration"
readonly prodBackendWithInstance="prod-backend-with-instance-$iteration"
readonly prodBackendService="prod-backend-service-$iteration"
readonly prodHealthCheck="prod-healthcheck-$iteration"
readonly prodStaticIP="prod-static-ip-$iteration"
readonly prodDB="prod-sql-$iteration"
readonly prodVM="prod-ubuntu-$iteration"
readonly prodBucket="gs://daemonprodbucket-$iteration"
readonly prodTemplate="prod-template-$iteration"
readonly prodInstanceGroupName="prod-instancegroup-$iteration"
readonly prodsslcert="daemonsipcert"
readonly prodGate80="prod-allow-80-$iteration"
readonly prodRedis="prod-redis-$iteration"
nbr_of_instances=2
readonly pathToSSLPrivKey="./rsakeys/sslprivatekey"
readonly pathToSSLCertification="./rsakeys/sslcertification"
#####general variables######
readonly pathToGitPrivKey="./rsakeys/gitprivkey"
readonly pathToGitPubKey="./rsakeys/gitpubkey"
readonly pathToGitAccessKey="./rsakeys/gitaccesskey"
readonly errorlog="./errorlog"
readonly log="./log"
readonly dbname="stemtestdb"
readonly dbusername="DAEMONS"
readonly namegcloudproject="cs2"
readonly gcloudacc=`gcloud projects list | grep $namegcloudproject | cut -f1 -d ' '`
readonly region="europe-west1"
# sql ips beeing used by application found in appsettings.json or appsettings.prod.json
readonly ipsqlstandard="34.77.106.120"
readonly ipsqlProduction="34.77.106.120"
readonly dbInUseUser="DAEMONS1"
readonly dbInUseUserPW="daemons1"
#where to install dotnet application on vm
readonly installationPath="/home/ubuntu"


#read into variables to use in startupscript
readonly gitPrivKey=`cat $pathToGitPrivKey`
readonly gitPubKey=`cat $pathToGitPubKey`
readonly gitAccess=`cat $pathToGitAccessKey`

# header and tail for errorlog banner
function Banner() {
if [ $1 == test ] ; then
testOrProd="test"
else testOrProd="production"
fi

if [ $2 == deploy ] ; then
deployOrDelete="deploy"
else deployOrDelete="delete"
fi

if [ $3 == start ] ; then
startOrStop="start"
else startOrStop="stop"
fi

errorBanner=$(cat <<EOF
============================================================================================
                               $iteration-$startOrStop-$testOrProd-$deployOrDelete-ERRORLOG-`date +"%d_%m_%Y-%T"`
============================================================================================
EOF
)

logBanner=$(cat <<EOF
============================================================================================
                                $iteration-$startOrStop-$testOrProd-$deployOrDelete-LOG-`date +"%d_%m_%Y-%T"`
============================================================================================
EOF
)

printf '%s\n' "$errorBanner" >> $errorlog
printf '%s\n' "$logBanner" >> $log

}


function readStartupScript() {
readonly startupscript=$(cat <<EOF
#!/bin/bash
apt-get update -y
#required installs general
apt-get install -y software-properties-common
apt-get install -y wget
apt-get install -y git
apt-get install -y node.js
apt-get install -y npm
apt-get install -y nginx
#librariers
apt-get install -y liblttng-ust0
apt-get install -y libcurl4
apt-get install -y libssl1.0.0
apt-get install -y libkrb5-3
apt-get install -y zlib1g
apt-get install -y libicu60
apt-get install -y libgdiplus
# dotnet installation
wget https://packages.microsoft.com/config/ubuntu/18.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
dpkg -i packages-microsoft-prod.deb
add-apt-repository universe
apt-get update -y
apt-get install -y apt-transport-https
apt-get install -y dotnet-sdk-3.1

#keys needed for gitclone
printf '%s\n' "$gitPubKey" > /root/.ssh/id_rsa.pub 2> ~/pubkeyerror
printf '%s\n' "$gitAccess" > /root/.ssh/known_hosts 2> ~/gitkeyerror
printf '%s\n' "$gitPrivKey" > /root/.ssh/id_rsa 2> ~/privkeyerror

#make keys secure to use
sudo chmod 600 /root/.ssh/id_rsa
sudo chmod 600 /root/.ssh/id_rsa.pub

#git clone the application into the installation path
cd $installationPath
git clone git@gitlab.com:kdg-ti/integratieproject-1/daemons/dotnet.git 2> ~/clone-errorlog

#change dbsettings in code so it uses cloud database
sed -i "s/$ipsqlstandard/$ipSQL/g" $installationPath/dotnet/UI-WebAPI/appsettings.json
sed -i "s/$dbInUseUser/$dbusername/g" $installationPath/dotnet/UI-WebAPI/appsettings.json
sed -i "s/$dbInUseUserPW/$dbuserpw/g" $installationPath/dotnet/UI-WebAPI/appsettings.json
sed -i "s/$ipsqlstandard/$ipSQL/g" $installationPath/dotnet/UI-MVC/appsettings.json
sed -i "s/$dbInUseUser/$dbusername/g" $installationPath/dotnet/UI-MVC/appsettings.json
sed -i "s/$dbInUseUserPW/$dbuserpw/g" $installationPath/dotnet/UI-MVC/appsettings.json

sed -i "s/$ipsqlProduction/$ipSQL/g" $installationPath/dotnet/UI-WebAPI/appsettings.Production.json
sed -i "s/$dbInUseUser/$dbusername/g" $installationPath/dotnet/UI-WebAPI/appsettings.Production.json
sed -i "s/$dbInUseUserPW/$dbuserpw/g" $installationPath/dotnet/UI-WebAPI/appsettings.Production.json
sed -i "s/$ipsqlProduction/$ipSQL/g" $installationPath/dotnet/UI-MVC/appsettings.Production.json
sed -i "s/$dbInUseUser/$dbusername/g" $installationPath/dotnet/UI-MVC/appsettings.Production.json
sed -i "s/$dbInUseUserPW/$dbuserpw/g" $installationPath/dotnet/UI-MVC/appsettings.Production.json

#stemtestdbInitializer
sed -i "s/\\\\\\\\\\\\\\DAL\\\\\\\\\\\\\\csv\\\\\\\\\\\\\\stemtest/\/DAL\/csv\/stemtest/g" $installationPath/dotnet/DAL/MySQL/StemtestDbInitializer.cs
sed -i "s/\\\\\\\\\\\\\\DAL\\\\\\\\\\\\\\csv\\\\\\\\\\\\\\woordenlijst/\/DAL\/csv\/woordenlijst/g" $installationPath/dotnet/DAL/MySQL/StemtestDbInitializer.cs

#change redisinfo
sed -i "s/redisinfo/$redisString/g" $installationPath/dotnet/UI-WebAPI/appsettings.json


#install npm mvc client
cd $installationPath/dotnet/UI-MVC/Client
npm install

#publish mvc/webapi
export DOTNET_CLI_HOME="$installationPath"

dotnet publish $installationPath/dotnet/UI-MVC/ -c release -o /var/www/mvc > ~/logmvc 2>> ~/publishmvcerrorlog
dotnet publish $installationPath/dotnet/UI-WebAPI/ -c release -o /var/www/webapi > ~/logwebap 2>> ~/publishapierrlog

#nginx configure configfile to redirect http:80 request to kestrelserver with correct port
#and allow use of signalR proxy headers for security
cat > nginxDefaultConfig <<'endmsg'

server {
    listen        80;
        server_name daemonstemtest.be www.daemonstemtest.be;
        proxy_hide_header X-Frame-Options;
        add_header X-XSS-Protection "1; mode=block" always;
        add_header X-Content-Type-Options "nosniff" always;
        add_header Referrer-Policy "no-referrer-when-downgrade" always;
        add_header Content-Security-Policy "default-src * data: 'unsafe-eval' 'unsafe-inline'" always;
        add_header Strict-Transport-Security "max-age=31536000; includeSubDomains; preload" always;

     location /api {
        proxy_pass http://localhost:5000;
    proxy_http_version 1.1;
    proxy_set_header Upgrade \$http_upgrade;
    proxy_set_header Connection \$http_connection;
    proxy_set_header Host \$host;
    proxy_cache_bypass \$http_upgrade;
    proxy_read_timeout 86400s;
    proxy_send_timeout 86400s;
}
location / {
        proxy_pass http://localhost:5001;
    proxy_http_version 1.1;
    proxy_set_header Upgrade \$http_upgrade;
    proxy_set_header Connection \$http_connection;
    proxy_set_header Host \$host;
    proxy_cache_bypass \$http_upgrade;
    proxy_read_timeout 86400s;
    proxy_send_timeout 86400s;
}
  }

endmsg

cat nginxDefaultConfig > /etc/nginx/sites-available/default
nginx -s reload

#systemctl stemtest-api.service config file
#Environments settings can be changed for development
cat > stemtest-api.service <<'endmsg'
[Unit]
Description=stemtest.webapi

[Service]
WorkingDirectory=$installationPath/dotnet/UI-WebAPI
ExecStart=/usr/bin/dotnet /var/www/webapi/UI-WebAPI.dll
Restart=always
# Restart service after 10 seconds if the dotnet service crashes
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=dotnet-api
#Environment=ASPNETCORE_ENVIRONMENT=Development
#Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false
[Install]
WantedBy=multi-user.target
endmsg


#systemctl stemtest-mvc.service config file
cat > stemtest-mvc.service <<'endmsg'
[Unit]
Description=stemtest.mvc

[Service]
WorkingDirectory=$installationPath/dotnet/UI-MVC
ExecStart=/usr/bin/dotnet /var/www/mvc/UI-MVC.dll
Restart=always
# Restart service after 10 seconds if the dotnet service crashes
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=dotnet-mvc
#Environment=ASPNETCORE_ENVIRONMENT=Development
#Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false
[Install]
WantedBy=multi-user.target
endmsg

cat stemtest-api.service > /etc/systemd/system/stemtest-api.service
cat stemtest-mvc.service > /etc/systemd/system/stemtest-mvc.service

#start api as service
systemctl enable stemtest-api.service
systemctl start stemtest-api.service
#start mvc as service
systemctl enable stemtest-mvc.service
systemctl start stemtest-mvc.service

EOF
)
}


###################################################################                   General functions                   ##########################################################
#                                                                                          start
####################################################################################################################################################################################

readonly help=$(cat <<EOF
NAME
deploy.sh:  the daemon integration project

SYNOPSIS
deploy.sh test [OPTION] | prod  [OPTION] | deleteall

OPTIONS
        -d --delete: delete everything related to the environment including backups
        -b --backup: makes a backup of the database of at the moment it is called
        -r --recover: restores the database with a chosen databasebackup
        --help: show the help screen
    (prod only)
        -s --size n: deploy n amount of application instances. Default 2

EOF
)

function help() {
echo "$help"
}

#check if necessary programs are installed
function checkInstalled() {
if ! [ -x "$(command -v gcloud)" ] ; then echo "gcloud not installed, program stopped" exit 1 ;fi
if ! [ -x "$(command -v mysql)" ] ; then echo "mysql not installed, program stopped" exit 1 ;fi
if ! [ -x "$(command -v gsutil)" ] ; then echo "gsutil not installed, program stopped" exit 1 ;fi
echo "checkInstalled Done"
}

function cleanMessage() {
if [ $1 != 0 ] ; then echo "ERROR: $2 not $3"
                        else echo "$2 successfully $3" ; fi
}

#creates backup of current database.
function Backup() {
        if [ $1 == test ] ; then
        bucket=$testBucket
        database=$testDB
        else
        bucket=$prodBucket
        database=$prodDB
        fi

        #create bucket
        gsutil mb -l europe-west1 -c standard -p $gcloudacc $bucket  2>>$errorlog
        #give rights to allow export
        serviceaccount=$(gcloud sql instances describe $database | grep serviceAccountEmailAddress: | cut -f2 -d ' ')
        gsutil iam ch serviceAccount:$serviceaccount:roles/storage.legacyBucketWriter $bucket
        #export database to bucket
        gcloud sql export sql $database $bucket/$(date +"%d_%m_%Y-%T")-sqldumpfile.gz --database=$dbname 2>>$errorlog
        echo "testBackup done"
}

#gives list with possible backups and restores chosen backup
function Restore() {
        #check if test or prod
        if [ $1 == test ] ; then
        bucket=$testBucket
        database=$testDB
        else
        bucket=$prodBucket
        database=$prodDB
        fi

        #check if backup/bucket exist
        gsutil ls $bucket > /dev/null 2>&1
        if [ $? != 0 ] ; then echo "No backups to restore" ; exit 1  ; fi
        #choose which backup
        echo "choose backup to restore by giving nr"
        count=0
        for backup in `gsutil ls $bucket` ; do
                ((count=count+1))
                echo "$count : $backup"
        done
        escape=false
        while [ "$escape" = false ] ; do
                read choice
                if ! [ "$choice" -eq "$choice" ] 2> /dev/null
                        then echo "not a valid choice, please give a nr"
                else
                        if [ $choice -gt $count ]
                                then  echo "not a valid choice try again"
                                else escape=true
                                dbtorestore=$(gsutil ls $bucket | sed -n "$choice p")
                        fi
                fi
        done

        #give rights to allow import
        serviceaccount=$(gcloud sql instances describe $database | grep serviceAccountEmailAddress: | cut -f2 -d ' ')
        gsutil iam ch serviceAccount:$serviceaccount:roles/storage.objectViewer $database
        #import sql db
        gcloud sql --quiet import sql $database $dbtorestore --database=$dbname >>$log  2>>$errorlog ; status=$?
        cleanMessage $status $database restored
}

#creates firewall rules with correct tag
function FirewallRules() {
if [ $1 == test ] ; then
tags="test-ubuntu"
name1=$testGate80
else tags="prod-ubuntu"
name1=$prodGate80
fi

gcloud compute firewall-rules create $name1 --allow=tcp:80 --target-tags=$tags >>$log 2>>$errorlog ; status=$?
        cleanMessage $status $name1 "deployed"

}

#starts an sql instance and fills it with users.
function StartSQLServer() {
#check if test or prod
if [ $1 == "test" ] ; then
        database=$testDB
        else
        database=$prodDB
fi

#create instance gcloud sql server
echo "Give root password (characters are hidden) and press enter"
read -s rootpw
#read dbuserpw
echo "Give password for $dbusername (characters are hidden) and press enter"
read -s dbuserpw
echo "Creating SQL instance, this might take a while"
gcloud sql instances create $database \
--region=$region \
--authorized-networks=0.0.0.0/0 \
--root-password=$rootpw >>$log 2>>$errorlog ; status=$?
        cleanMessage $status $database "deployed"

#get ip from gcloud sql instance
ipSQL="$(gcloud sql instances describe $database | grep ipAddress: | cut -f 3 -d ' ')"
#add database/users to sql server with all privileges
mysql --host $ipSQL --user=root -p$rootpw -e "CREATE DATABASE $dbname;
CREATE USER '$dbusername' IDENTIFIED BY '$dbuserpw';
GRANT ALL PRIVILEGES ON *.* TO '$dbusername'@'%';

#CREATE USER 'DBONDERHOUD' IDENTIFIED BY 'dbonderhoud';
#GRANT ALL PRIVILEGES ON *.* TO 'DBONDERHOUD'@'%';
" >>$log 2>>$errorlog
}



###################################################################                Test-environment-specific-functions                ##############################################
#
####################################################################################################################################################################################


#deletes all test envirnment instances
function testDelete() {
	echo "---start deleting test---"
	Banner test delete start
	gcloud sql instances delete -q $testDB 2>>$errorlog ; status=$?
        cleanMessage $status $testDB "deleted"
	gcloud compute instances delete -q $testVM 2>>$errorlog ; status=$?
        cleanMessage $status $testVM "deleted"
        gcloud compute --quiet firewall-rules delete  $testGate80 2>>$errorlog ; status=$?
        cleanMessage $status $testGate80 "deleted"
	gsutil rm -r $testBucket 2>>$errorlog ; status=$?
	cleanMessage $status $testBucket "deleted"
	Banner test delete stop
	echo "---end deleting test---"
}

#starts the test environment application server
function testStartAppServer() {
gcloud compute instances create $testVM \
--image-project=ubuntu-os-cloud \
--image-family=ubuntu-1804-lts \
--tags test-ubuntu \
--metadata startup-script="$startupscript"
} >>$log 2>>$errorlog

################################################################### 	         Production-environment-specific-functions           ###############################################
#
####################################################################################################################################################################################

#starts the production environment application servers
function prodStartAppServer() {

#make template for instance group
gcloud compute instance-templates create $prodTemplate \
--image-project=ubuntu-os-cloud \
--image-family=ubuntu-1804-lts \
--tags prod-ubuntu \
--metadata startup-script="$startupscript" >>$log  2>>$errorlog ; status=$?
	cleanMessage $status $prodTemplate "deployed"
#Create instance groups
gcloud compute instance-groups managed create $prodInstanceGroupName \
--region $region \
--template $prodTemplate \
--size $nbr_of_instances >>$log  2>>$errorlog ; status=$?
	cleanMessage $status $prodInstanceGroupName "deployed"

##create static ip
#gcloud compute addresses create $prodStaticIP \
#--ip-version=IPV4 \
#--global >>$log  2>>$errorlog ; status=$?
#cleanMessage $status $prodStaticIP "deployed"

#create health check
gcloud compute health-checks create http $prodHealthCheck \
--port 80 >>$log  2>>$errorlog ; status=$?
cleanMessage $status $prodHealthCheck "deployed"

#create backend service
gcloud compute backend-services create $prodBackendService \
--protocol HTTP \
--session-affinity=GENERATED_COOKIE \
--timeout=3600 \
--health-checks $prodHealthCheck \
--global >>$log  2>>$errorlog ; status=$?
	cleanMessage $status $prodBackendService "deployed"

#long timeout needed for signalR

#add instance group to backend services
gcloud compute backend-services add-backend $prodBackendService \
--instance-group $prodInstanceGroupName \
--instance-group-region=$region \
--balancing-mode=UTILIZATION \
--max-utilization=0.8 \
--global >>$log  2>>$errorlog ; status=$?
        cleanMessage $status $prodBackendService "deployed"

#create url map
gcloud compute url-maps create $prodWebMap \
--default-service $prodBackendService >>$log  2>>$errorlog ; status=$?
        cleanMessage $status $prodWebMap "deployed"

#create sslcerts
#gcloud compute  ssl-certificates create $prodsslcert \
#--certificate=$pathToSSLCertification \
#--private-key=$pathToSSLPrivKey \
#--global >>$log  2>>$errorlog ; status=$?
#        cleanMessage $status $prodsslcert "deployed"

#create http proxy to link url
gcloud compute target-https-proxies create $prodHttpProxy  \
--url-map=$prodWebMap \
--ssl-certificates $prodsslcert  >>$log  2>>$errorlog ; status=$?
        cleanMessage $status $prodHttpProxy "deployed"

#give ip backend #--address=daemonsip \
gcloud compute forwarding-rules create $prodForwardingRule \
--address=daemonsip \
--global \
--target-https-proxy=$prodHttpProxy \
--ports=443 >>$log  2>>$errorlog ; status=$?
        cleanMessage $status $prodForwardingRule "deployed"

}


#limit access to database to IPs from VMs
function prodLimitSQLAccess() {
gcloud sql --quiet  instances patch $prodDB --authorized-networks `gcloud compute instances list | grep $prodInstanceGroupName | tr -s ' '  | cut -d' ' -f5 | tr '\n' ',' | sed '$ s/.$//'` >>$log  2>>$errorlog
status=$?
cleanMessage $status limitSQLAccess "deployed"

}

#create redis server to support signalR with multiple vm instances
function prodCreateRedis() {
gcloud redis instances create $prodRedis --region=$region >>$log  2>>$errorlog ; status=$?
        cleanMessage $status $prodRedis "deployed"

#read in values to insert into startupscript
ipRedis=`gcloud redis instances describe $prodRedis --region=$region | grep host: | cut -f2 -d' '`
portRedis=`gcloud redis instances describe $prodRedis --region=$region | grep port  | cut -f2 -d' '`
redisString="$ipRedis:$portRedis"
}


#deletes all the instances of the production environment
function prodDelete() {
	echo "---start deleting prod---"
        Banner prod delete start
	gcloud sql instances delete -q $prodDB >>$log  2>>$errorlog ; status=$?
	cleanMessage $status $prodDB "deleted"
	gcloud compute --quiet forwarding-rules delete $prodForwardingRule --global >>$log  2>>$errorlog ; status=$?
        cleanMessage $status $prodForwardingRule "deleted"
	gcloud compute --quiet target-https-proxies delete $prodHttpProxy >>$log  2>>$errorlog ; status=$?
        cleanMessage $status $prodHttpProxy "deleted"
#	gcloud compute --quiet ssl-certificates delete $prodsslcert >>$log  2>>$errorlog ; status=$?
#        cleanMessage $status $prodsslcert "deleted"
	gcloud compute --quiet url-maps delete $prodWebMap >>$log  2>>$errorlog ; status=$?
        cleanMessage $status $prodWebMap "deleted"
	gcloud compute --quiet backend-services delete $prodBackendService --global >>$log  2>>$errorlog ; status=$?
        cleanMessage $status $prodBackendService "deleted"
	gcloud compute --quiet health-checks delete $prodHealthCheck >>$log  2>>$errorlog ; status=$?
        cleanMessage $status $prodHealthCheck "deleted"
#	gcloud compute --quiet addresses delete $prodStaticIP --global >>$log 2>>$errorlog ; status=$?
#       cleanMessage $status $prodStaticIP "deleted"
	gcloud compute --quiet instance-groups managed delete $prodInstanceGroupName --region=europe-west1 >>$log  2>>$errorlog ; status=$?
        cleanMessage $status $prodInstanceGroupName "deleted"
	gcloud compute --quiet instance-templates delete $prodTemplate >>$log  2>>$errorlog ; status=$?
        cleanMessage $status $prodTemplate "deleted"
	gcloud compute --quiet firewall-rules delete  $prodGate80 >>$log  2>>$errorlog ; status=$?
        cleanMessage $status $prodGate80 "deleted"
	gcloud redis --quiet instances delete $prodRedis --region=$region >>$log  2>>$errorlog ; status=$?
        cleanMessage $status $prodRedis "deleted"
        gsutil rm -r $prodBucket >>$log  2>>$errorlog ; status=$?
        cleanMessage $status $prodBucket "deleted"
	Banner prod delete stop
	echo "---end deleting prod---"
}


###################################################################                Entrypoint script                   ##########################################################
#
####################################################################################################################################################################################


testOrProd=$1 #valid options: prod / help / test /deleteall

#select optional parameters for test environment and/or run test functions
if [ ${testOrProd:-"test"} = "test" ] ; then

	if [ $# -gt 2 ] ; then
		echo "max 1 parameter"
		exit 1
	fi

	if [ $# -eq 2  ] ; then
		case $2 in
		-d | --delete )	testDelete
				exit 0;;
		-b | --backup)  Backup test
				exit 0;;
		-r | --restore) Restore test
				exit 0;;
		--help) 	help
				exit 0;;
		*) echo "not a valid parameter, try -d -b -r"
				exit 1;;
		esac
    		shift
	fi
echo "---start deploy test---"
Banner test deploy start
checkInstalled
FirewallRules test
StartSQLServer test
readStartupScript
testStartAppServer
echo `gcloud compute instances describe $testVM | grep natIP`
Banner test deploy stop
echo "---deploy test finished---"
exit 0
fi


if [ ${testOrProd} = "--help" ] ; then
	help
	exit 0
fi

#select optional parameters for production environment and/or run application
if [ ${testOrProd} = "prod" ] ; then
	#check if arguments are valid
	if [ $# -gt 3 ] 2>>$errorlog ; then
		echo "---too many arguments---"
		help
		exit 1
	fi
	if [[ $2 == "-s"  || $2 == "--size" ]] ; then
	echo "---amount of vm's beeing made: $3---"
	else
		if [ $# -gt 2 ] ; then
			echo "---too many arguments---"
                	help
                	exit 1
		fi
	fi
	#select parameters and run code
	while [ "$1" != "" ]; do
		case $1 in
			-d | --delete )	prodDelete
					exit 0 ;;
			-b | --backup) Backup Prod
					exit 0 ;;
			-r | --restore) Restore prod
					exit 0 ;;
			--help) help
				exit 0 ;;
			-s | --size ) if [ "$2" -eq "$2" ] 2>>$errorlog ; then
					nbr_of_instances=$2
					else
					echo "after -s or --size a number needs to be given"
					exit 1
				fi
		esac
    		shift
	done
	echo "---start deploy production---"
	Banner production deploy start
	checkInstalled
	FirewallRules prod
	StartSQLServer prod
	prodCreateRedis
	readStartupScript
	prodStartAppServer
	prodLimitSQLAccess
	Banner production deploy stop
	echo "---deploy production finished---"
	exit 0
fi

#delete all instances made
if [ ${testOrProd} = "deleteall" ] ; then
	if [ $# -gt 1 ] ; then
                echo "---too many arguments---"
                help
                exit 1
	fi
	testDelete
	prodDelete
	exit 0
fi

echo "parameter one needs to be test/prod/deleteall/--help"
exit 1

