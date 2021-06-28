# Integratieproject 1 &nbsp;&nbsp;&nbsp;  2019-2020
## Daemons

- Ace De Jong
- Arthur De Craemer
- Elias Malomgré
- Jarre Michiels
- Seppe Van de Poel
- Tim Schelpe


## Korte beschrijving

.NET core N-layer applicatie.

## Lange beschrijving

Dit is de .NET core applicatie voor het integratieproject.
Deze solution is uitgewerkt volgens de N-layer architectuur en omvat de volgende projecten.
- DAL: Toegang tot de databank en queries.
- BL: Logica omtrent het spel en toegangspunt tot de DAL.
- Domain: Domeinmodel uitgewerkt in code.
- UI-MVC: De webapplicatie waar men het spel op kan spelen en gebruik kan maken van de functionaliteiten voor leerkrachten, admins, superadmins,..
- UI-WebAPI: Requests voor de android applicatie verwerken en signalR voor zowel android en UI-MVC bijhouden.

## Instructies

### UI-MVC

#### Opstarten

De UI-MVC applicatie start op zonder bijkomende configuratie. Om de volledige functionaliteit te kunnen gebruiken wanneer men lokaal runt moet er wel het volgende gebeuren.
1. UI-WebAPI moet runnen (mag ook na het opstarten MVC).
2. UI-WebAPI runt op poort 5000 (kestrel via https) dit is normaal ook de enige startup configuratie voor de WebAPI.

##### Inloggen

Er staan een aantal testgegevens klaar om de functionaliteiten van verschillende soorten gebruikers te testen.

###### Leerkracht

Voor de leerkracht staan al een aantal klassen klaar. U kan meteen beginnen met een sessie te starten.
- Email/username: teacher@kdg.be
- Wachtwoord: teacher123

of

- Email/username: arthur@kdg.be
- Wachtwoord: arthur123

###### Admin/School

De administrator beheert de bovenstaande leerkrachten. U kan dus meteen de functionaliteit van het beheren van leerkrachten testen. Ook het gebruik van dit account als leerkracht is mogelijk.

- Email/username: admin@kdg.be
- Wachtwoord: admin123

###### Superadmin

De superadmin beheert bovenstaande admin en kan ook alle functionaliteit van een admin en leerkracht gebruiken.

- Email/username: superadmin@treecompany.be
- Wachtwoord: superadmin123

###### Leerling of school die een account wilt aanvragen

Een school die een account wilt aanvragen kan op de navigatiebalk van de login pagina op "For schools" navigeren. Er wordt dan een mail naar de superadmin gestuurd met de gegevens van de school zodat deze de aanvraag kan bekijken.

LET OP: deze mail wordt om te kunnen testen naar arthur.decraemer@student.kdg.be gestuurd en niet naar de email van het dummy superadmin account. Als u bewijs wilt van deze functionaliteit kan u een aanvraag sturen en een teamlid om bewijs vragen.

Een leerling kan terecht op de Game pagina (default pagina bij start). En vanuit daar met een sessiecode starten




