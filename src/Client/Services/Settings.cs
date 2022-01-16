using System.Collections.Generic;

namespace MessengerWeb.Client.Services
{
    public class Settings
    {
        public List<EngineEntity> Engines { get; set; }

        public Settings()
        {
            Engines = new List<EngineEntity> 
            {
                new EngineEntity()
                {
                    Name = "Luna Platform",
                    Id = "fea041df-4e7e-4e59-ae9a-68a4500a1754",
                    IsSelected = true,
                },
                new EngineEntity()
                {
                    Name = "NTech Find Face",
                    Id = "bla-bla",
                    IsSelected = false,
                },
                new EngineEntity()
                {
                    Name = "Tevian Platform",
                    Id = "",
                    IsSelected = false,
                }
            };
        }
    }
}
