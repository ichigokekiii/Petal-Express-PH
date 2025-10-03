// File: ~/Scripts/HolyScripts/Service.js
app.factory('authService', function () {
    var authData = {};

    // This is our shared array for registered users
    authData.users = [];

    // This is the shared login status that the header will watch
    authData.isLoggedIn = false;

    // Function to add a new user to our array
    authData.registerUser = function (user) {
        // Simple validation to check if user already exists
        for (var i = 0; i < authData.users.length; i++) {
            if (authData.users[i].email === user.email) {
                alert('This email is already registered.');
                return; // Stop the function
            }
        }
        authData.users.push(angular.copy(user));
        alert('Registration successful!');
    };

    // Function to check credentials and log in
    authData.login = function (credentials) {
        for (var i = 0; i < authData.users.length; i++) {
            if (authData.users[i].email === credentials.email && authData.users[i].password === credentials.password) {
                authData.isLoggedIn = true;
                return true; // Login successful
            }
        }
        return false; // Login failed
    };

    return authData;
});