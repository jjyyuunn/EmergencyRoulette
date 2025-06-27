namespace EmergencyRoulette
{
    public class PlayerResource
    {
        public int Energy;
        public int Food;
        public int Medical;
        public int Data;

        public float OverloadGauge; // 0 ~ 100%

        public PlayerResource()
        {
            Energy = 0;
            Food = 0;
            Medical = 0;
            Data = 0;
            OverloadGauge = 0f;
        }
    }
}