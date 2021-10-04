# DevicesManager

A lightweight system to create, edit, delete and list devices.

## Getting Started

You have two fronted, Angular Web and console.
They are 3 storage available, memory, file or LiteDB.
Console can work with API and Queue (only for create, edit and delete)

## Prerequisites

RabbitMQ - for queues

.NET 5.0

## Configuration

### API:

Storage:

In appsettings you can configure wich storage you want two use, for that you must create a section named Storage.
Inside we have two properties, Type and Filename.
Type can be: Mempory, File or LiteDB.
Filename is only used for File and LiteDB.

Let see an example for LiteDB:

```
"Storage": {
  "Type": "LiteDB",
  "Filename": "devices.db"
}
```

Queue:

In appsettings you can enable the RabbitMQ queue, for that you must create a section named Queue.
The section have 5 properties, Host, Port, QueueName, Username and Password.
Username and password are optional (by default guest will be used).

Let see an example 

```
"Queue": {
  "Host": "localhost",
  "Port": 5672,
  "QueueName": "devices"
}
```

### Console:

Backend:

In appsettings you must create a section named Backend.
Inside we have 3 properties, Type, HostAPI and Queue.
Type can be: API or Queue.
HostAPI is mandatory even if you choose Queue type.
Queue have the followed properties: Host, Port, QueueName, Username and Password.
Username and password are optional (by default guest will be used).

Let see an example for Queue:

```
"Backend": {
  "Type": "Queue",
  "HostAPI": "http://localhost:8001",
  "Queue": {
    "Host": "127.0.0.1",
    "Port": 5672,
    "QueueName":  "devices"
  }
}
```

## Deployment

You can deploy the DeviceManager.Web project on linux as service and configure reverse proxy on nginx or you can also use the Dockerfile to build a container.

### As Service:

- Build the app:

On console go to the project folder of DeviceManager.Web and run the command:

```
dotnet publish -c release -o publish -r linux-x64
```

That will compile and generate the output files in the publish folder.

- Upload:

Upload the files on your linux server in for example /var/netcore/DevicesManager.Web and give permisions to the user www-data.

- Create & Enable the service:

On server go to /etc/systemd/system and create a file name DevicesManagerWeb.service with the following content:

```
[Unit]
Description=Lightweight system to create, edit, delete and list devices.

[Service]
WorkingDirectory=//var/netcore/DevicesManager.Web
ExecStart=/usr/bin/dotnet /var/netcore/DevicesManager.Web/DevicesManager.Web.dll
Restart=always
# Restart service after 10 seconds if the dotnet service crashes:
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=dotnet-DevicesManagerWeb
User=www-data
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false

[Install]
WantedBy=multi-user.target
```

Then run the commands for enable and start the service:

```
sudo systemctl enable DevicesManagerWeb.service
sudo systemctl start DevicesManagerWeb.service
```

You can check if the service is running with the command:

```
sudo systemctl status DevicesManagerWeb.service
```

Now we must configure the Nginx, for that go to /etc/nginx/sites-enabled and create a file named devicesmanagerweb with the followed content:

```
server {
    listen        80;
    location / {
        proxy_pass         http://127.0.0.1:5000;
        proxy_http_version 1.1;
        proxy_set_header   Upgrade $http_upgrade;
        proxy_set_header   Connection keep-alive;
        proxy_set_header   Host $host;
        proxy_cache_bypass $http_upgrade;
        proxy_set_header   X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header   X-Forwarded-Proto $scheme;
    }
}
```

Reload nginx config using this command 

```
sudo nginx -s reload
```

### Docker:

The solution have a Dockerfile for create a container with the Web project.

On terminal go inside the solution path and run this command:

```
docker build -t devicemanagerweb .
```

That will create the container image.

For run it just type:

```
docker run -p 8001:80 -d devicemanagerweb
```
