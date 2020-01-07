# ConsoleApp13

A concept of automatic worker activation/deactivation, depending on the queue state. When it's empty, then worker count is decreased every 5 seconds, and if it's non empty, worker count is increased to a maximum value every 5 seconds. I think it can be useful for you, because in this case the number of workers on each server will be minimum, and more servers will be able to pick up jobs. When more background jobs is added, more workers will be spawned to handle the load.

The code is available in this repository with a humble name – https://github.com/odinserj/ConsoleApp13.

The main classes start with Gate and contain a wrapper for the Worker process that respect the leveling feature, GateTuner background process that increases or decreases the current gate level, and Gate class itself. All of those classes are wrapped together in the CustomBackgroundJobServer class that you can use instead of BackgroundJobServer one.

It's based on three following classes. Also there's the CustomBackgroundJobServer class that wraps everything, and only 3 lines are different from the BackgroundJobServer class.

* Gate – the main logic that controls the number of active workers, depending on the current "level" that's increased or decreased externally.
* GateWorker – wrapper for the Worker class, uses gate to prevent or allow the processing.
* GateTuner – checks the given queues to see whether there are pending jobs. If they are empty, gate level will be decreased. If there are pending work items, gate level will be increased to allow more workers to process them.

Please note it currently works on a best-effort basis, and actually there may be more workers than specified in the gate's level. But once one of them processes a new job, the number of active workers is decreased toward to the correct value.
