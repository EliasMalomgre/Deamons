Feature: SelecteerPartij
	Om mijn kennis over een partij te kunnen testen
	Wil ik als leerling
	een partij kunnen selecteren


Scenario: Selecteer VB
	Given ik leerlingId 0  van klas "203A" in de sessie "PTest" van leerkracht 69
	When Ik mijn partij "Vlaams Belang" selecteer
	And het spel is gestart
	Then Dan zou ik hiervan het partijspel moeten krijgen
