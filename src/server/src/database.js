const sqlite3 = require('sqlite3').verbose();
const db = new sqlite3.Database('./data/weather.db'); 

const day_seconds = 86400;


// Define the formatUnixTimestamp function
function formatUnixTimestamp(unixTimestamp) {
  const timestampInMillis = unixTimestamp * 1000;
  const date = new Date(timestampInMillis);

  const year = date.getFullYear();
  const month = String(date.getMonth() + 1).padStart(2, '0');
  const day = String(date.getDate()).padStart(2, '0');
  const hours = String(date.getHours()).padStart(2, '0');
  const minutes = String(date.getMinutes()).padStart(2, '0');
  const seconds = String(date.getSeconds()).padStart(2, '0');

  return `${year}-${month}-${day} ${hours}:${minutes}:${seconds}`;
}


function formatUnixTimestampInLocalTime(unixTimestamp) {
  console.log(unixTimestamp)
  const utcTime = new Date(unixTimestamp * 1000); // Convert seconds to milliseconds
  // const localTime = utcTime.toLocaleString(); // Use default options for local time
  
  console.log(utcTime)
  // Convert to local time and format it
  const localTimeString = utcTime.toLocaleString(undefined, {
    hour: 'numeric',
    minute: 'numeric',
    hour12: false, // Use 24-hour format
  });

  console.log(localTimeString)


  return localTimeString;
}


function createDatabase() {
    db.serialize(() => {
      db.run(`
        CREATE TABLE IF NOT EXISTS daq (
          time REAL,
          temp REAL,
          humidity REAL
        )
      `);
    });
  }

function getDatabaseData(callback) {
    const query = 'SELECT * FROM daq ORDER BY rowid DESC LIMIT 1;';
  
    db.get(query, (err, row) => {
      if (err) {
        console.error(err);
        callback(err, null);
      } else {
        callback(null, row);
      }
    });
  }

  function getLast24Data(callback) {
    const query = 'SELECT strftime("%H:%M", time, "unixepoch") AS ts, temp, humidity FROM daq WHERE time BETWEEN ? AND ? ORDER BY time ASC;';
    const upperTimeStampLimit = Math.floor(Date.now() / 1000); // Divide by 1000 to convert milliseconds to seconds
    const lowerTimeStampLimit = upperTimeStampLimit - day_seconds;
  
    db.all(query, [lowerTimeStampLimit, upperTimeStampLimit], (err, rows) => {
      if (err) {
        console.error(err.message);
        callback(err, null);
        return;
      }
      
      const timestamps = [];
      const data1 = [];
      const data2 = [];
  
      rows.forEach(row => {
        // Convert epoch time (UTC) to local time
        // Create a Date object with the UTC time
        const utcTime = new Date(`1970-01-01T${row.ts}Z`);

        // Convert to local time and format it
        // Convert to local time and format it
        const localTimeString = utcTime.toLocaleString(undefined, {
          hour: 'numeric',
          minute: 'numeric',
          hour12: false, // Use 24-hour format
        });
        
        timestamps.push(localTimeString);
        data1.push(row.temp);
        data2.push(row.humidity);
      });
  
      const chartData = {
        labels: timestamps,
        datasets: [
          {
            label: 'Data 1',
            data: data1,
            // Additional configuration for the data 1 dataset (e.g., colors, options)
          },
          {
            label: 'Data 2',
            data: data2,
            // Additional configuration for the data 2 dataset (e.g., colors, options)
          }
        ]
      };
  
      callback(null, chartData);
    });
  }
  



  
  


function storeWeatherData(input1, input2, callback) {
    const unixTimestamp = Math.floor(Date.now() / 1000); // Divide by 1000 to convert milliseconds to seconds

  
    const query = 'INSERT INTO daq (time, temp, humidity) VALUES (?,?,?)';
    db.run(query, [unixTimestamp,input1, input2], function (err) {
      if (err) {
        console.error(err);
        callback(err);
      } else {
        const insertedId = this.lastID;
        callback(null, insertedId);
      }
    });
  
  }
  
  module.exports = {
    createDatabase,
    getDatabaseData,
    storeWeatherData,
    formatUnixTimestamp,
    getLast24Data,
    formatUnixTimestampInLocalTime
  };
  
  