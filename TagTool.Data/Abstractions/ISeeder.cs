namespace TagTool.Data.Services
{
    /* 
    The abstraction of Seeder, allows the service to be injected through 
    dependency injection.
    */
    public interface ISeeder
    {
        void Seed();
    }
}