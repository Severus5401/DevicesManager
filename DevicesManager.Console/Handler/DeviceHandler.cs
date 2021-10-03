using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DevicesManager.Console.Backend;
using DevicesManager.Core.Models.Requests;
using DevicesManager.Core.Models.Responses;
using Microsoft.Extensions.Hosting;

namespace DevicesManager.Console.Handler
{
    public class DeviceHandler : BackgroundService, IHostedService
    {
        private const string EXIT_VALUE = "q!";
        private readonly IBackend _backend;

        public DeviceHandler(IBackend backend)
        {
            _backend = backend;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                await _backend.GetSupportedDevices();
            }
            catch (Exception e)
            {
                WriteLine($"The backend is not available, please check the configuration and that the backend is up. {Environment.NewLine}Details: {e.Message}");
                return;
            }

            try
            {
                await doJob();
            }
            catch (Exception e)
            {
                WriteLine($"An unexpected error occurs: {e.Message}");
            }
        }

        private async Task doJob()
        {
            try
            {
                WriteLine("Welcome to the Device Manager");
                string result;

                do
                {
                    PrintMenu();

                    result = ReadLine.Read();

                    if (result == "1")
                        await printDevices();
                    else if (result == "2")
                        await createDevice();
                    else if (result == "3")
                        await updateDevice();
                    else if (result == "4")
                        await deleteDevice();
                    else
                        if (result != EXIT_VALUE)
                            WriteLine("You have entered an invalid option.");

                } while (result != EXIT_VALUE);
            }
            catch (Exception e)
            {
                WriteLine($"An unexpected error occurs: {e.Message}");
            }
        }

        private void PrintMenu()
        {
            WriteLines(new List<string>() {
                "What do you want to do:",
                "[1] - List all devices.",
                "[2] - Create new device.",
                "[3] - Update a device.",
                "[4] - Delete a device.",
                $"[{EXIT_VALUE}] - Exit."
            });
        }

        private async Task printDevices()
        {
            try
            {
                var devices = await _backend.List();
                if (devices.Any())
                    foreach (var device in devices)
                        WriteLine(System.Text.Json.JsonSerializer.Serialize(device));
                else
                    WriteLine("No devices found");
            }
            catch(Exception e)
            {
                WriteLine($"An error occurs: {e.Message}");
            }
        }

        private async Task createDevice()
        {
            try
            {
                var request = new DeviceRequest();

                exitInstruction();

                WriteLine("Supported devices type:");

                var types = await _backend.GetSupportedDevices();
                foreach (var type in types)
                    WriteLine($"[{type.Id}] - {type.Value}");

                WriteLine("");
                bool validDeviceType = false;
                do
                {
                    request.DeviceType = TypedInput<short>("Select a device type: ", true);
                    validDeviceType = types.Any(x => x.Id == request.DeviceType);
                    if (!validDeviceType)
                        WriteLine("The device type specified is not supported.");

                } while (!validDeviceType);

                request.SerialNumber = FreeInput("Introduce a Serial Number: ", true);
                request.FirmwareVersion = FreeInput("Introduce a Firmware Verison: ", false, request.FirmwareVersion);
                request.State = FreeInput("Introduce a State:", false, request.State);

                if (request.DeviceType == (int)EDeviceType.Gateway)
                {
                    request.IP = FreeInput("Introduce an IP:", false, request.IP);
                    request.Port = TypedInput<int>("Introduce a Port (numeric only): ", false);
                }

                await _backend.Add(request);

                WriteLine($"Device successfully created");
            }
            catch (OperationExitException)
            {
                return;
            }
            catch(Exception e)
            {
                WriteLine($"An error occurs: {e.Message}");
            }
        }

        private async Task updateDevice()
        {
            try
            {
                exitInstruction();

                DeviceResponse device;

                do
                {
                    string id = FreeInput("Please introduce the device id: ", true);
                    device = await _backend.GetById(id);

                    if (device == null)
                    {
                        WriteLine($"No device found with the id: {id}");
                    }
                } while (device == null);
                    

                var request = new DeviceRequest()
                {
                    DeviceType = Convert.ToInt16(device.DeviceType),
                    FirmwareVersion = device.FirmwareVersion,
                    Id = device.Id,
                    SerialNumber = device.SerialNumber,
                    State = device.State,
                    IP = device.IP,
                    Port = device.Port
                };

                request.FirmwareVersion = FreeInput($"Introduce a Firmware Verison [{request.FirmwareVersion}]: ", false, request.FirmwareVersion);
                request.State = FreeInput($"Introduce a State [{request.State}]: ", false, request.State);

                if (request.DeviceType == (int)EDeviceType.Gateway)
                {
                    request.IP = FreeInput($"Introduce an IP [{request.IP}]: ", false, request.IP);
                    device.Port = TypedInput<int>($"Introduce a Port (numeric only) [{device.Port}]: ", false, device.Port.ToString());
                }

                if (await _backend.Update(request))
                    WriteLine($"Device successfully updated");
                else
                    WriteLine($"The device could not be updated");

            }
            catch (OperationExitException)
            {
                return;
            }
            catch (Exception e)
            {
                WriteLine($"An error occurs: {e.Message}");
            }
        }

        private async Task deleteDevice()
        {
            try
            {
                exitInstruction();

                string id = FreeInput("Please introduce the device id: ", true);

                if (await _backend.Delete(id))
                    WriteLine($"Device successfully deleted.");
                else
                    WriteLine($"No device found with the given id: {id}");
            }
            catch (OperationExitException)
            {
                return;
            }
            catch (Exception e)
            {
                WriteLine($"An error occurs: {e.Message}");
            }
        }


        private void exitInstruction()
        {
            WriteLine($"To abort the operation please type: {EXIT_VALUE}");
        }

        private void WriteLines(IEnumerable<string> lines)
        {
            foreach (var line in lines)
                WriteLine(line);
        }

        private void WriteLine(string line)
        {
            System.Console.WriteLine(line);
        }

        private string FreeInput(string prompt, bool mandatory = false, string defaultValue = "")
        {
            string result;
            bool exit;
            do
            {
                result = ReadLine.Read(prompt, defaultValue);

                checkOperationExit(result);

                exit = !(string.IsNullOrEmpty(result) && mandatory);

                if (!exit)
                    WriteLine("This field is mandatory");

            } while (!exit);

            return result;
        }

        private T TypedInput<T>(string prompt, bool mandatory = false, string defaultValue = "") 
        {
            do
            {
                var value = FreeInput(prompt, mandatory, defaultValue);

                if (string.IsNullOrEmpty(value) && !mandatory)
                    return default(T);

                var converter = TypeDescriptor.GetConverter(typeof(T));
                if (converter != null)
                {
                    try
                    {
                        return (T)converter.ConvertFromString(value);
                    }
                    catch
                    {
                        WriteLine("Invalid value.");
                    }
                }
            } while (true);
        }

        private void checkOperationExit(string value)
        {
            if (value == EXIT_VALUE)
                throw new OperationExitException("User cancel current operation");
        }

       
    }
}
