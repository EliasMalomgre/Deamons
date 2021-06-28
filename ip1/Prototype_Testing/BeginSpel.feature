Feature: BeginSpel
	Om een spel te beginnen
	Wil ik als leerling
	Een leerlingsessie starten.


Scenario: Correcte start
	Given Ik leerling met id 0  wil de test "PTest" met klas "203A" doen over "Vlaams Belang" van leerkracht 69
	When Ik mijn systeem initialiseer
	Then Moet mijn sessie aangemaakt zijn
