const express = require('express');
const jwt = require('jsonwebtoken');
const tokenStore = require('../tokenStore');
const router = express.Router();

const JWT_SECRET = 'your_secret_key';
const JWT_REFRESH_SECRET = 'your_refresh_secret_key';

const users = [
  { id: 1, username: 'user1', password: '123456', email: 'user1@gmail.com', role: 'user' },
  { id: 2, username: 'user2', password: 'abcdef', email: 'user2@gmail.com', role: 'admin' }
];

router.post('/login', (req, res) => {
  const { username, password } = req.body;
  const user = users.find(u => u.username === username && u.password === password);
  if (!user) return res.status(401).json({ message: 'Invalid credentials' });

  const accessToken = jwt.sign(
    { id: user.id, username: user.username, role: user.role },
    JWT_SECRET,
    { expiresIn: '1h' }
  );
  const refreshToken = jwt.sign(
    { id: user.id, username: user.username, role: user.role },
    JWT_REFRESH_SECRET,
    { expiresIn: '7d' }
  );
  tokenStore.add(refreshToken);

  res.json({
    accessToken,
    refreshToken,
    user: {
      id: user.id,
      username: user.username,
      email: user.email,
      role: user.role
    }
  });
});

// Route để làm mới access token
router.post('/refresh', (req, res) => {
  const { refreshToken } = req.body;
  if (!refreshToken || !tokenStore.exists(refreshToken)) {
    return res.status(403).json({ message: 'Refresh token not found, login again' });
  }
  jwt.verify(refreshToken, JWT_REFRESH_SECRET, (err, user) => {
    if (err) return res.status(403).json({ message: 'Invalid refresh token' });
    const accessToken = jwt.sign(
      { id: user.id, username: user.username, role: user.role },
      JWT_SECRET,
      { expiresIn: '1h' }
    );
    res.json({ accessToken });
  });
});

// Route logout (xóa refresh token)
router.post('/logout', (req, res) => {
  const { refreshToken } = req.body;
  tokenStore.remove(refreshToken);
  res.json({ message: 'Logged out successfully' });
});

module.exports = router; 