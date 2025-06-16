const express = require('express');
const authenticateToken = require('../middleware/auth');
const router = express.Router();

const users = [
  { id: 1, username: 'user1', email: 'user1@gmail.com', role: 'user', fullName: 'User One' },
  { id: 2, username: 'user2', email: 'user2@gmail.com', role: 'admin', fullName: 'User Two' }
];

router.get('/profile', authenticateToken, (req, res) => {
  const user = users.find(u => u.id === req.user.id);
  if (!user) return res.status(404).json({ message: 'User not found' });
  res.json(user);
});

module.exports = router; 