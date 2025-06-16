const express = require('express');
const bodyParser = require('body-parser');
const loginRoute = require('./routes/login');
const profileRoute = require('./routes/profile');

const app = express();
app.use(bodyParser.json());

app.use('/api', loginRoute);
app.use('/api', profileRoute);

app.listen(5037, () => {
  console.log('Server running on port 5037');
}); 