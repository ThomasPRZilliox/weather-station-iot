const express = require('express');
const bodyParser = require('body-parser');
const app = express();

const { createDatabase, getDatabaseData, storeWeatherData, formatUnixTimestamp, getLast24Data, formatUnixTimestampInLocalTime } = require('./src/database');


app.set('view engine', 'ejs');
app.set('views', __dirname + '/views');

app.use('/favicon.ico', express.static('public/images/favicon.ico'));

app.use(express.static('public'));
app.use(bodyParser.json());

createDatabase(); // Create the database if it doesn't exist


app.get('/', (req, res) => {
  getDatabaseData((err, data) => {
    if (err) {
      res.status(500).send('Internal Server Error');
    } else {
      getLast24Data((err2, data2) => {
        if (err2) {
          res.status(500).send('Internal Server Error');
        } else {
          res.render('home', { data, formatUnixTimestamp, data2 });
        }
      });
    }
  });
});



app.get('/contact', (req, res) => {
  res.send('This is the contact page');
});

app.post('/daq', (req, res) => {
  const { temp, humidity } = req.body;

  storeWeatherData(temp, humidity, (err, insertedId) => {
    if (err) {
      console.error(err);
      res.status(500).send('Error storing data in database');
    } else {
      res.json({ id: insertedId });
    }
  });
});

const port = 3000;
const host = '0.0.0.0'; // Listen on all network interfaces
app.listen(port, host, () => {
  console.log(`Server is running on ${host}:${port}`);
});   