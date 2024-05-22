
using System;
using System.Collections.Generic;

namespace AnodyneArchipelago
{
    internal class Locations
    {
        public static Dictionary<Guid, string> LocationsByGuid = new()
        {
            {new Guid("1525ead8-4d92-a815-873a-4dc04037d973"), "Street - Broom Chest" },
            {new Guid("3307aa58-ccf1-fb0d-1450-5af0a0c458f7"), "Street - Key Chest" },
            {new Guid("1bcddffb-5f98-4787-3a81-d1ce8fdeca29"), "Overworld - Near Gate Chest" },
            {new Guid("40de36cf-9238-f8b0-7a57-c6c8ca465cc2"), "Temple of the Seeing One - Entrance Chest" },
            {new Guid("621c284f-cbd0-74c3-f51b-2a9fdde8d4d7"), "Temple of the Seeing One - Rock-Surrounded Chest" },
            {new Guid("401939a4-41ba-e07e-3ba2-dc22513dcc5c"), "Temple of the Seeing One - Dark Room Chest" },
            {new Guid("88d0a7b8-eeab-c45f-324e-f1c7885c41ce"), "Temple of the Seeing One - Shieldy Room Chest" },
            {new Guid("c481ee20-662e-6b02-b010-676ab339fc2d"), "Temple of the Seeing One - Health Cicada" },
            {new Guid("0bce5346-48a2-47f9-89cb-3f59d9d0b7d2"), "Temple of the Seeing One - Boss Chest" },
            {new Guid("d41f2750-e3c7-bbb4-d650-fafc190ebd32"), "Temple of the Seeing One - After Statue Left Chest" },
            {new Guid("3167924e-5d52-1cd6-d6f4-b8e0e328215b"), "Temple of the Seeing One - After Statue Right Chest" },
        };
    }
}
