using System;
using System.Collections.Generic;
using System.Linq;
using Stemtest.BL.Domain.Test;

namespace Stemtest.DAL
{
    public class PartijRepoHC: IPartijRepository
    {
        private Dictionary<string, Partij> repo;

        public PartijRepoHC()
        {
            repo = new Dictionary<string, Partij>();
            initialise();
        }
        public Partij Read(string naam)
        {
            Partij partij = repo[naam];
            if (partij == null)
            {
                throw new Exception("Partij niet gevonden");
            }

            return partij;
        }

        public void Create(Partij partij)
        {
            repo[partij.naam] = partij;
        }

        public void Update(Partij partij)
        {
            if (Read(partij.naam) != null)
            {
                repo[partij.naam] = partij;
            }
            else
            {
                throw new Exception("Partij niet gevonden");
            }
        }

        public void Delete(string naam)
        {
            bool isGelukt = repo.Remove(naam);
            if (!isGelukt)
            {
                throw new Exception("Partij niet gevonden");
            }
        }

        public List<Partij> ReadAllePartijen()
        {
            //yikes
            return repo.Values.ToList();
        }

        private void initialise()
        {
            Partij VlaamschBlock = new Partij("Vlaams Belang","Extreem rechts","Donker Geel","Tom van Grieken","Vlaamse_Leeuw.jpg");
            VlaamschBlock.opinies.Add(new Antwoord(1,1,"nee","Er kan beter geïnvesteerd worden in kernenergie"));
            VlaamschBlock.opinies.Add(new Antwoord(2,2,"ja","Wij geloven dat de gemeentes zelf moeten kunnen kiezen of ze er willen of niet. zij staan namelijk het dichtst bij de burger die er de lasten van ondervind."));
            VlaamschBlock.opinies.Add(new Antwoord(3,3,"ja","De burgers betalen al meer dan genoeg, niet alle burgers moeten betalen omdat enkele burgers zich niet aan de afspraken houden. Verder zijn onze straten proper genoeg."));
            VlaamschBlock.opinies.Add(new Antwoord(4,4,"ja","Kernenergie is een goedkope en propere bron van energie. Ook is de huidige infrastructuur al aanwezig en hebben we gezien wat een kernenergie uitstap betekend voor de omgeving, bv in Duitsland"));
            VlaamschBlock.opinies.Add(new Antwoord(5,5,"ja","Wij geloven dat je maar een goede burger kunt zijn als je de taal spreekt. Deze test zou ervoor moeten zorgen dat iedereen zijn steentje bijdraagt aan de samenleving"));
            VlaamschBlock.opinies.Add(new Antwoord(6,6,"ja","In moeilijke tijden moet het mogelijk zijn om extreme maatregelen te nemen, bijvoorbeeld in het rood gaan. Om zo het probleem op te lossen."));
            VlaamschBlock.opinies.Add(new Antwoord(7,7,"nee","We werken al lang genoeg, wij geloven dat we het geld ergens anders moeten gaan halen. Immigratie bijvoorbeeld.  Op je 65 verdien je een waardig pensioen"));
            VlaamschBlock.opinies.Add(new Antwoord(8,8,"nee","Wij zijn niet tegen mobiliteit, maar de budgetten daarvoor zijn al hoog genoeg. Mensen verplaatsen zich vandaag al genoeg, dit moet dus niet aangemoedigd worden."));
            VlaamschBlock.opinies.Add(new Antwoord(9,9,"ja","Ja, dit zorgt voor een extra bescherming van de verhuurder. Die vandaag de dag teveel moet opdraaien voor slechte huurders"));
            VlaamschBlock.opinies.Add(new Antwoord(10,10,"ja","Ja, het is logisch dat je kinderen de bezittingen krijgen waar je je leven lang voor hebt gewerkt. De overheid zou hier niet tussen mogen komen."));
            repo.Add(VlaamschBlock.naam,VlaamschBlock);
            
            Partij Gruun = new Partij("Groen","Ni links genoeg","Groen","Meyrem Almaci","geel.jpg");
            Gruun.opinies.Add(new Antwoord(1,1,"ja","Er kan beter geïnvesteerd worden in kernenergie"));
            Gruun.opinies.Add(new Antwoord(2,2,"nee","Wij geloven dat de gemeentes zelf moeten kunnen kiezen of ze er willen of niet. zij staan namelijk het dichtst bij de burger die er de lasten van ondervind."));
            Gruun.opinies.Add(new Antwoord(3,3,"nee","De burgers betalen al meer dan genoeg, niet alle burgers moeten betalen omdat enkele burgers zich niet aan de afspraken houden. Verder zijn onze straten proper genoeg."));
            Gruun.opinies.Add(new Antwoord(4,4,"nee","Kernenergie is een goedkope en propere bron van energie. Ook is de huidige infrastructuur al aanwezig en hebben we gezien wat een kernenergie uitstap betekend voor de omgeving, bv in Duitsland"));
            Gruun.opinies.Add(new Antwoord(5,5,"nee","Wij geloven dat je maar een goede burger kunt zijn als je de taal spreekt. Deze test zou ervoor moeten zorgen dat iedereen zijn steentje bijdraagt aan de samenleving"));
            Gruun.opinies.Add(new Antwoord(6,6,"nee","In moeilijke tijden moet het mogelijk zijn om extreme maatregelen te nemen, bijvoorbeeld in het rood gaan. Om zo het probleem op te lossen."));
            Gruun.opinies.Add(new Antwoord(7,7,"ja","We werken al lang genoeg, wij geloven dat we het geld ergens anders moeten gaan halen. Immigratie bijvoorbeeld.  Op je 65 verdien je een waardig pensioen"));
            Gruun.opinies.Add(new Antwoord(8,8,"ja","Wij zijn niet tegen mobiliteit, maar de budgetten daarvoor zijn al hoog genoeg. Mensen verplaatsen zich vandaag al genoeg, dit moet dus niet aangemoedigd worden."));
            Gruun.opinies.Add(new Antwoord(9,9,"nee","Ja, dit zorgt voor een extra bescherming van de verhuurder. Die vandaag de dag teveel moet opdraaien voor slechte huurders"));
            Gruun.opinies.Add(new Antwoord(10,10,"nee","Ja, het is logisch dat je kinderen de bezittingen krijgen waar je je leven lang voor hebt gewerkt. De overheid zou hier niet tussen mogen komen."));
            repo.Add(Gruun.naam,Gruun);
        }
    }
}