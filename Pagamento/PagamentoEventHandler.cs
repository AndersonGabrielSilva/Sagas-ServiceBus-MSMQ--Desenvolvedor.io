using System;
using System.Threading.Tasks;
using Core.Messages.IntegrationEvents;
using Pagamento.Commands;
using Rebus.Bus;
using Rebus.Handlers;

namespace Pagamento
{
    public class PagamentoEventHandler :
        IHandleMessages<PedidoRealizadoEvent>
    {
        private readonly IBus _bus;

        public PagamentoEventHandler(IBus bus)
        {
            _bus = bus;
        }

        public Task Handle(PedidoRealizadoEvent message)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("REALIZANDO PAGAMENTO!");
            Console.ForegroundColor = ConsoleColor.Black;

            // O comando é uma intenção de modificação o Evento não.
            // por este motivo não podemos aplicar a regra de negocio aqui
            _bus.Send(new RealizarPagamentoCommand { AggregateRoot = message.AggregateRoot }).Wait();

            return Task.CompletedTask;
        }
    }
}