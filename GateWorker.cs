// This file is part of Hangfire.
// Copyright © 2019 Sergey Odinokov.
// 
// Hangfire is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as 
// published by the Free Software Foundation, either version 3 
// of the License, or any later version.
// 
// Hangfire is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public 
// License along with Hangfire. If not, see <http://www.gnu.org/licenses/>.

using System;
using Hangfire.Logging;
using Hangfire.Server;

namespace ConsoleApp13
{
    public class GateWorker : IBackgroundProcess
    {
        private readonly ILog _logger = LogProvider.GetCurrentClassLogger();
        private readonly Gate _gate;
        private readonly Worker _innerWorker;

        public GateWorker(Gate gate, Worker innerWorker)
        {
            _gate = gate ?? throw new ArgumentNullException(nameof(gate));
            _innerWorker = innerWorker ?? throw new ArgumentNullException(nameof(innerWorker));
        }

        public void Execute(BackgroundProcessContext context)
        {
            _logger.Trace("Acquiring a gate...");
            _gate.Wait(context.StoppingToken);
            _logger.Trace("Gate acquired.");

            try
            {
                _innerWorker.Execute(context);
            }
            finally
            {
                _logger.Trace("Releasing gate");
                _gate.Release();
            }
        }
    }
}