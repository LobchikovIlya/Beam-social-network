window.decodeJwt = function (token) {
    const payload = JSON.parse(atob(token.split('.')[1]));
    return payload;
}
