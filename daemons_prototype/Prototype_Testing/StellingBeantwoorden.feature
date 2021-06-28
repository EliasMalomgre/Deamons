Feature: StellingBeantwoorden
	Om mijn antwoord te laten registreren
	wil ik als leerling
	mijn een stelling beantwoorden.

Scenario: correcte invoer
Given ik op stelling 1 zit als leerling 0 in de sessie van leerkracht 69
When ik "ja" aanduid 
And argumentering "" ingeef
Then word mijn antwoord geregistreerd
