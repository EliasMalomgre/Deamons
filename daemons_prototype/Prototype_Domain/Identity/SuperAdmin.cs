namespace Prototype_Domain.Identity
{
    public class SuperAdmin: Admin
    {
        
        
        public SuperAdmin(string persoonNaam, string organisatieNaam) : base(persoonNaam, organisatieNaam)
        {
        }

        public void MaakPartij()
        {
            //Hmmmmm
            //not sure dat die methode bij de superadmin hoort ipv bij partij
            //also, kan niet uitwerken zonder dal services
        }
    }
}