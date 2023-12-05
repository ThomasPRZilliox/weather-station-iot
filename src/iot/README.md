<div align="center">
    <img src="./iot-svgrepo-com.svg" alt="Logo" width="160" height="160">
</div>

# IOT

The Raspberry Pico will function as an Internet of Things device, transmitting acquired Humidity and Temperature data to a server at a frequency of 0.1 Hz.

## Configure the app

Look for the lines and update the url of your server:
 ```
 # Send the data to Raspberry PI server
            url = '<>:3000/daq'
 ```
For example:
 ```
 # Send the data to Raspberry PI server
            url = 'http://192.168.1.74:3000/daq'
 ```

Update the file [config.json](./config.json) accordingly to be allow your pico to connect to your internet, in the thingspeak_api key add the write API key of your thingspeak channel.

By default the acquisition is made every 10 seconds in case of success, if you want to change that setting look for the lines : 

```
# Wait 10 minute in case of success
        time.sleep(600)
```

## Setup the Raspberry Pico WH
- Install the Thonny application
- Make sure your Raspberry Pico is setup (that MicroPyython is installed). If not put your device in bootloader mode (hold the BOOTSEL button while pluging in it to your computer. Go to the Thonny app and click on the button on the bottom right corner, then select "Install MicroPython..."
- Copy the IOT directory on the Raspberry
- Run the code an ensure that it's acquiring data and sending it to the server
- Deploy the code on the Raspberry using CTRL+D
- Plug the Raspberry on a power source and leave it running

