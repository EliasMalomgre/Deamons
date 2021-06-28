# Integratieproject 1 &nbsp;&nbsp;&nbsp;  2019-2020
## Daemons

- Ace De Jong
- Arthur De Craemer
- Elias Malomgré
- Jarre Michiels
- Seppe Van de Poel
- Tim Schelpe


## Korte beschrijving

Android applicatie om educatieve stemtest mee te doorlopen.

## Lange beschrijving

De android applicatie die bedoeld is om de Educatieve stemtest mee te spelen. 
Deze applicatie werkt minimaal op API level 21 en vereist internet en camera permissies.
De app communiceert met het WebAPI project van de educatieve stemtest. 
De applicatie maakt gebruik van een single activity pattern, alle verschillende schermen zijn fragments die via de navcontroller op de Main Activity worden attached.
We maken gebruik van Room om de data van 1 spel te cachen. Deze data word verwijdert wanneer men probeert te connecteren met een nieuw spel.
Retrofit wordt gebruikt om API calls te doen. Alle asynchrone operaties in onze app gebeuren via RXJava2.
Ook wordt er gebruik gemaakt van ViewModels. We werken met het Model-view-viewmodel architecturaal patroon. 


## Instructies

#### Installatie

Je kan de app runnen door via Android Studio een Android emulator van minimaal API level 21 te gebruiken.
Voor je de app runt check je best na of het IP/Poort waarop verbonden wordt overeen komt met het IP/Poort van de WebAPI(default poort 5000). Het te verbinden Ip vind je in een constante "ip" in  kdg.be.stemtest.DI.modules.NetworkModule.kt .

Als deze op localhost runt moet je in de emulator ip 10.0.2.2 gebruiken, deze staat gemapt op de localhost van je computer. 
Je kan via android studio de app ook op een smartphone installeren of je kan via android studio een APK maken voor installatie. Indien je een echte smartphone gebuikt kan je niet met localhost werken.


