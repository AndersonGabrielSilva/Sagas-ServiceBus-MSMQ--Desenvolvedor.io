using System;
using System.Threading.Tasks;
using Core.Messages.IntegrationEvents;
using Rebus.Handlers;

namespace Pedido
{
    public class PedidoEventHandler :
        IHandleMessages<PedidoRealizadoEvent>
    {

        //Este evento pode ser manipulado por varios handlers, não necessariamente somente pelo pedidoEventHabndler
        //como por exemplo o "PedidoRealizadoEvent", ele é utilizado lá em Pedido Saga Tambem
        public Task Handle(PedidoRealizadoEvent message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("PEGUEI EM OUTRO LUGAR");
            Console.ForegroundColor = ConsoleColor.Black;
            return Task.CompletedTask;
        }
    }
}