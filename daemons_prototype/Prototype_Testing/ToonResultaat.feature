Feature: ToonResultaat
	Om te weten of ik goed gescoord heb
	wil ik als leerling
	mijn resultaat bekijken.


Scenario: Resultaat bekijken
	Given ik ben leerling 0 
	And ik zit in klas "203A"
	And ik neem deel aan de "PTest" sessie van leerkracht 69
	And ik heb "Vlaams Belang" gekozen
	And de sessie is gestart 
	And ik 6 juiste antwoorden heb ingevuld
	When ik mijn resultaat wilt bekijken
	Then dan zal mijn score 6 zijn op de test
	