# Integratieproject 1 &nbsp;&nbsp;&nbsp;  2019-2020
## Daemons

- Ace De Jong
- Arthur De Craemer
- Elias Malomgré
- Jarre Michiels
- Seppe Van de Poel
- Tim Schelpe


### Korte beschrijving

Het Deployment script voor de educatieve stemtest.


### Algemene informatie
Het `deploy.sh` script is gemaakt voor om een .NETCORE applicatie te deployen. 
Er zijn 2 versies, namelijk test en production. In de testversie wordt er 1 ubuntu 18.04 lts instantie, 1 sql database instantie en eventueel een bucket instantie via het Google Cloud Platform (GCP) geïnstantieerd. 
In de productie versie wordt er 1 instantiegroep met 1-n (standaard 2) ubuntu 18.04 lts instanties, 1 loadbalancer, 1 sql database en eventueel een bucket via het GCP geïnstantieerd. 
Van opstart tot werkende site kan het 15 minuten duren.

### Om  het `deploy.sh` script te gebruiken via uw ubuntu
##### Moet de volgende software geïnstalleerd zijn
* google cloud SDK 
https://cloud.google.com/sdk/docs#install_the_latest_cloud_tools_version_cloudsdk_current_version
* gsutil
https://cloud.google.com/storage/docs/gsutil_install
* MySQL
https://www.digitalocean.com/community/tutorials/how-to-install-mysql-on-ubuntu-18-04

##### Moeten de volgende bestanden aanwezig zijn:

*Voor het gebruik van self signed certificates*
* ssl private key 
* sslcertificate 
#
*Om een git clone te kunnen uitvoeren*
* private key
* public key
* git access keys

https://docs.gitlab.com/ee/ssh/
Volg de stappen in bovenstaande bron voor git clone te kunnen uitvoeren.
##### Verdere vereisten
Google Cloud Platform (GCP) moet krediet hebben om te werken.
Deze account moet ook een default region en zone hebben ingesteld.

Bovenaan het script staan er variabelen die veranderen per persoon, deze moeten vervangen worden door uw eigen gegevens. Bijvoorbeeld:
- prodsslcert
- pathToGitPrivKey
- pathToGitPubKey
- ...

#### Verdere informatie:
GCP laat niet toe om sql instances met dezelfde naam te hebben, zelfs na verwijderen. Verander daarom bij elke run de volgende variabele. (geen hoofdletters toegelaten)
```
readonly iteration="eigenwaarde"
```
#
STDERR en STDOUT worden in *errorlog* en *log* file geschreven in de directory waar `deploy.sh` staat. 
#
In de huidige versie wordt gebruik gemaakt van google self signed certificate dat op voorhand geproduceert is. 
Moest u zelf zo een certificaat hebben verandert u enkel de *prodsslcert* naar uw eigen certificaatnaam.
Indien u dit niet heeft haalt u in de *prodStartAppServer* en *prodDelete* functies de volgende lijnen uit commentaar:
#
```
#gcloud compute addresses create $prodStaticIP \
#--ip-version=IPV4 \
#--global >>$log  2>>$errorlog ; status=$?
#       deploy $status $prodStaticIP

#gcloud compute  ssl-certificates create $prodsslcert \
#--certificate=$pathToSSLCertification \
#--private-key=$pathToSSLPrivKey \
#--global >>$log  2>>$errorlog ; status=$?
#        deploy $status $prodWebMap

#gcloud compute --quiet addresses delete $prodStaticIP --global >>$log 2>>$errorlog ; status=$?
#delete $status $prodStaticIP

#gcloud compute --quiet addresses delete $prodStaticIP --global >>$log 2>>$errorlog ; status=$?
#delete $status $prodStaticIP
```

#### Hoe gebruiken:
`deploy.sh` test [ARGUMENT] | prod  [ARGUMENT] | deleteall
 ARGUMENTEN
        -d --delete: Verwijder alles van deze omgeving, inclusief backups.
        -b --backup: Maakt een backup van de databank op de moment dat hij aangeroepen wordt.
        -r --recover: Geeft een lijst van bruikbare backups, de gekozen backup wordt gebruikt.
        --help: Laat help zien op het scherm.
        Enkel voor productie omgeving:
            -s --size n: Deploy n aantal applicatie instanties, default is 2.