import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html'
})
export class HomeComponent {
  public devices: Device[];
  public deviceTypes: DeviceType[];
  public request: DeviceRequest = new DeviceRequest();
  public status: string;
  public errorMessage: string;

  private http: HttpClient;
  private baseUrl: string;

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {

    this.http = http;
    this.baseUrl = baseUrl;

    this.http.get<DeviceType[]>(this.baseUrl + 'Devices/SupportedDevices').subscribe(result => {
      this.deviceTypes = result;
    }, error => console.error(error));

    this.loadDevices();
  }

  public createDevice() {
    this.http.post(this.baseUrl + 'Devices', this.request).subscribe({
      next: data => {
        this.status = 'Device created successfully';
        this.request = new DeviceRequest();
        this.loadDevices();
        setTimeout(() => {
          this.clearStatus();
        }, 3000);
      },
      error: error => {
        this.errorMessage = error.error;
      }
    });
    this.loadDevices();
  }

  public deleteDevice(id: string) {
    this.http.delete(this.baseUrl + 'Devices/' + id)
      .subscribe({
        next: data => {
          this.status = 'Device deleted successfully';
          this.loadDevices();
          setTimeout(() => {
            this.clearStatus();
          }, 3000);
        },
        error: error => {
          this.errorMessage = error.error;
        }
      });
  }

  private clearStatus() {
    this.status = '';
    this.errorMessage = '';
  }

  private loadDevices() {
    this.http.get<Device[]>(this.baseUrl + 'Devices').subscribe(result => {
      this.devices = result;
    }, error => console.error(error));
  }
}

interface Device {
  id: string;
  serialNumber: string;
  deviceType: string;
  firmwareVersion: string;
  state: string;
  ip: string;
  port: number;
}

interface DeviceType {
  id: number;
  value: string;
}

export class DeviceRequest {
  id: string = '';
  serialNumber: string = '';
  deviceType: number = 0;
  firmwareVersion: string = '';
  state: string = '';
  ip: string = '';
  port: number = 0;
}
