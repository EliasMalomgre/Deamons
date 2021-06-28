Feature: WoordUitleg
	Om te begrijpen wat een woord betekent
	Wil ik als leerling
	Zodat meer uitleg willen kunnen vragen


Scenario: meer uitleg
	Given Ik ben leerling 0 
	And Ik zit in klas "203A"
	And Ik neem deel aan de "PTest" sessie van leerkracht 69
	And Ik heb "Vlaams Belang" gekozen
	And De sessie is gestart 
	And Ik heb 0 stellingen beantwoord
	When Ik moeilijke woorden opvraag
	Then Moet ik een uitleg krijgen
