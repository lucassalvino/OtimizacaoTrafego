using System;

namespace Simulador.Util
{
    public class RoletaSorteio
    {
        public static bool ExecutaRoleta(int porcentagem)
        {
            Random value = new Random(DateTime.Now.Millisecond);
            return value.Next() % 100 <= porcentagem;
        }
    }
}
