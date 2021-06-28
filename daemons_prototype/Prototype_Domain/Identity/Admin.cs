namespace Prototype_Domain.Identity
{
    public class Admin: Leerkracht
    {
        public Organisatie Organisatie { get; set; }
        
        public Admin(string persoonNaam, string schoolNaam): base(persoonNaam)
        {
            this.Organisatie = new Organisatie(schoolNaam);
        }
    }
}