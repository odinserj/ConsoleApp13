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
using System.Threading;

namespace ConsoleApp13
{
    public class Gate
    {
        private readonly object _syncRoot = new object();
        private int _current;
        private int _level;

        public Gate(int minLevel, int maxLevel)
        {
            _level = minLevel;

            MinLevel = minLevel;
            MaxLevel = maxLevel;
        }

        public int MinLevel { get; }
        public int MaxLevel { get; }

        public bool TryIncreaseLevel(out int level)
        {
            lock (_syncRoot)
            {
                if (_level < MaxLevel)
                {
                    level = _level += 1;
                    Monitor.Pulse(_syncRoot);
                    return true;
                }

                level = MaxLevel;
                return false;
            }
        }

        public bool TryDecreaseLevel(out int level)
        {
            lock (_syncRoot)
            {
                if (_level > MinLevel)
                {
                    level = _level -= 1;
                    return true;
                }

                level = MinLevel;
                return false;
            }
        }

        public void Wait(CancellationToken token)
        {
            lock (_syncRoot)
            {
                while (_current >= _level)
                {
                    token.ThrowIfCancellationRequested();
                    Monitor.Wait(_syncRoot, TimeSpan.FromSeconds(1));
                }

                _current++;
            }
        }

        public void Release()
        {
            lock (_syncRoot)
            {
                if (_current > 0)
                {
                    _current--;
                    Monitor.Pulse(_syncRoot);
                }
            }
        }
    }
}