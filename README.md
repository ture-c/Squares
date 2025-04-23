Förord
Jag vet att man skulle använda REACT. Jag har inte lärt mig det ännu, men kommer att ha gjort detta till sommaren. Läste inte heller square-logiken i programmet. 


Så API:t fungerar med ett enkelt REST Web api i ASP.NET core som hanterar rutor. med position x,y och föärg. Den använder en lokal json-fil som datalagring. Personligen använder jag mer databaser med EF core vanligtvis. 
Huvudfunktionerna är att den hämtar alla rutor(GET), lägger till en ny ruta(POST), Rensar alla rutor(DELETE).

Frontend integrerar med backend genom, vid laddning av sidan kör fetchboxes() som gör en GET till boxes och hämtar alla rutor och renderar dem i canvasen med hjälp av javascript. När användaren klickar på "lägg till ruta" så skapas ett nytt objekt med en position + färg och skickar en post request. API returnerar det sparade opbjektet som läggs till i boxes[]-listan och renderas direkt. 
clearallboxes() skickar DELETE och rensar boxes[] och tömmer canvasen. 
