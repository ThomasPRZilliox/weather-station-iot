import machine
import urequests 
from machine import Pin
import network, time
from dht import DHT11, InvalidChecksum
import json

led=machine.Pin('LED', machine.Pin.OUT)
led.on()
time.sleep(2)
led.off()

HTTP_HEADERS = {'Content-Type': 'application/json'}


## Read config data
# Open the JSON file for reading
with open('config.json', 'r') as f:
    # Load the JSON data from the file into a Python dictionary
    data = json.load(f)


# Access the values of the keys
password = data['password']
wifi = data['wifi']
thingspeak_api = data['thingspeak_api']


# Configure Pico W internet connection

wlan = network.WLAN(network.STA_IF)
wlan.active(True)
wlan.connect(wifi,password)

 
while True:
    pin = Pin(0, Pin.OUT, Pin.PULL_DOWN)
    sensor = DHT11(pin)
    try:
        t  = (sensor.temperature)
        h = (sensor.humidity)
        print("Temperature: {} and Humidity: {}".format(sensor.temperature,sensor.humidity))
        if wlan.isconnected():
            
            # Send the data to thingspeak cloud to ensure redundancy.
            dht_readings = {'field1':t, 'field2':h}
            try:
                request = urequests.post( 'https://api.thingspeak.com/update?api_key=' + thingspeak_api, json = dht_readings, headers = HTTP_HEADERS,timeout=5 )  
                request.close()
            except Exception as e:
                print("An exception occurred during the data transfer to thingspeak: {}".format(e))
                
            # Send the data to Raspberry PI server
            url = '<>:3000/daq'
            data = {'temp': t, 'humidity': h}
            try:
                response = urequests.post(url, json=data,timeout=10)
                response.close()
            except Exception as e:
                print("An exception occurred during the data transfer to raspberry pi: {}".format(e))
        # Wait 10 minute in case of success
        time.sleep(600)
        
    except Exception as e:
        print("An exception occurred : {}".format(e))
        # Wait 5 seconds in case of failure
        time.sleep(5)
        

