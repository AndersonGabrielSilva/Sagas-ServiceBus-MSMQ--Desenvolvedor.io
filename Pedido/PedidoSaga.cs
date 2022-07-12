using System;
using System.Threading.Tasks;
using Core.Messages.IntegrationEvents;
using Pedido.Commands;
using Rebus.Bus;
using Rebus.Handlers;
using Rebus.Sagas;

namespace Pedido
{
    #region Aanotaçoes sobre Saga
    /* Saga:
       Saga só faz zentido em um ambiente de microserviçoes.
     */
    #endregion


    /// <summary>
    /// Saga nada mais é do que um Process Manager (Gerenciador de Processos).
    /// </summary>
    public class PedidoSaga : Saga<PedidoSagaData>,
        IAmInitiatedBy<RealizarPedidoCommand>, //IAmInitiatedBy<T> => Eu sou iniciado pelo CommandX : Inicio da Saga
        IHandleMessages<PagamentoRealizadoEvent>,
        IHandleMessages<PedidoFinalizadoEvent>,
        IHandleMessages<PagamentoRecusadoEvent>,
        IHandleMessages<PedidoCanceladoEvent>

    {
        private readonly IBus _bus;

        public PedidoSaga(IBus bus)
        {
            _bus = bus;
        }

        protected override void CorrelateMessages(ICorrelationConfig<PedidoSagaData> config)
        {
            /* A saga trabalha sempre em cima de um ID
               toda Saga tem um ID, que agrupa todos os processos referente a uma unica saga
             */

            // Define como ID da Saga o AggregateRoot
            // Caso esquecer de fazer isto o Rebus nem funciona
            config.Correlate<RealizarPedidoCommand>(m => m.AggregateRoot, d => d.Id);
            config.Correlate<PagamentoRealizadoEvent>(m => m.AggregateRoot, d => d.Id);
            config.Correlate<PedidoFinalizadoEvent>(m => m.AggregateRoot, d => d.Id);
            config.Correlate<PagamentoRecusadoEvent>(m => m.AggregateRoot, d => d.Id);
            config.Correlate<PedidoCanceladoEvent>(m => m.AggregateRoot, d => d.Id);
        }

        public Task Handle(RealizarPedidoCommand message)
        {
            //_bus.Send(new RealizarPedidoCommand{AggregateRoot = message.AggregateRoot}).Wait();
            // Realiza processamento de negocio!

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Pedido Realizado!");
            Console.ForegroundColor = ConsoleColor.Black;

            _bus.Publish(new PedidoRealizadoEvent { AggregateRoot = message.AggregateRoot }).Wait();
            Data.PedidoRealizado = true;

            ProcessoSaga();

            return Task.CompletedTask;
        }

        public Task Handle(PagamentoRealizadoEvent message)
        {
            //_bus.Send(new FinalizarPedidoCommand { AggregateRoot = message.AggregateRoot }).Wait();
            // Realiza processamento de negocio!

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Pagamento Realizado!");
            Console.ForegroundColor = ConsoleColor.Black;

            _bus.Publish(new PedidoFinalizadoEvent { AggregateRoot = message.AggregateRoot }).Wait();
            Data.PagamentoRealizado = true;

            ProcessoSaga();

            return Task.CompletedTask;
        }

        public Task Handle(PedidoFinalizadoEvent message)
        {
            Data.PedidoFinalizado = true;

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Pedido FINALIZADO!");
            Console.ForegroundColor = ConsoleColor.Black;

            ProcessoSaga();

            return Task.CompletedTask;
        }

        public Task Handle(PagamentoRecusadoEvent message)
        {
            //_bus.Send(new CancelarPedidoCommand { AggregateRoot = message.AggregateRoot }).Wait();
            // Realiza processamento de negocio!

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Pagamento Recusado!");
            Console.ForegroundColor = ConsoleColor.Black;

            _bus.Publish(new PedidoCanceladoEvent() { AggregateRoot = message.AggregateRoot }).Wait();
            Data.PagamentoRealizado = false;

            ProcessoSaga();

            return Task.CompletedTask;
        }

        public Task Handle(PedidoCanceladoEvent message)
        {
            Data.PedidoCancelado = true;

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Pedido CANCELADO!");
            Console.ForegroundColor = ConsoleColor.Black;

            ProcessoSaga();

            return Task.CompletedTask;
        }

        public void ProcessoSaga()
        {
            if (Data.SagaCompleta)
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("Finalizando Saga!");
                Console.ForegroundColor = ConsoleColor.Black;

                // Informa que a Saga foi completa.
                MarkAsComplete();
            }
        }
    }

    /* 
     * SagaData : É utilizado para realizar o controle de todo o processo da saga
     * atraves das propriedades boleanas presente na classe de saga
     * 
     */
    public class PedidoSagaData : SagaData
    {
        public bool PedidoRealizado { get; set; }
        public bool PagamentoRealizado { get; set; }
        public bool PedidoFinalizado { get; set; }
        public bool PedidoCancelado { get; set; }

        public bool SagaCompleta => PedidoRealizado
                                 && PagamentoRealizado
                                 && PedidoFinalizado 
                                 || PedidoCancelado;
    }
}