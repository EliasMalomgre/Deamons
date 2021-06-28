Feature: InitialiseerSysteem
	Om het spel te kunnen gaan spelen
	Wil ik als leerkracht
	het systeem kunnen initialiseren.


Scenario: correcte initialisatie
	Given ik leerkracht 69 wil de test "PTest" met klas "203A" doen over "Vlaams Belang"
	When ik mijn systeem initialiseer
	Then moet mijn sessie aangemaakt zijn