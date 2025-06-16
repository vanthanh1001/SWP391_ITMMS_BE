// Lưu refresh token tạm thời (nên dùng DB thực tế)
const refreshTokens = [];

module.exports = {
  add: (token) => refreshTokens.push(token),
  remove: (token) => {
    const idx = refreshTokens.indexOf(token);
    if (idx > -1) refreshTokens.splice(idx, 1);
  },
  exists: (token) => refreshTokens.includes(token)
}; 