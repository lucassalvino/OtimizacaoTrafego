using System.Runtime.Serialization;

namespace Simulador.Entidades.Enuns
{
    public enum EstadosSemaforo
    {
        [EnumMember(Value = "0")]
        FECHADO = 0,
        [EnumMember(Value = "1")]
        ABERTO = 1,
        [EnumMember(Value = "2")]
        AMARELO = 2
    }
}
