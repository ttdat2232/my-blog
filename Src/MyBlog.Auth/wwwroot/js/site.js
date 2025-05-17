// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

document.addEventListener("DOMContentLoaded", function () {
  const form = document.querySelector(".login-form");
  if (form) {
    form.addEventListener("submit", function (e) {
      const username = form.querySelector('input[name="UsernameOrEmail"]');
      const password = form.querySelector('input[name="Password"]');
      const sessionIdInput = form.querySelector('input[name="SessionId"]');
      if (!username.value.trim() || !password.value.trim()) {
        e.preventDefault();
        alert("Please enter both username/email and password.");
        return;
      }
      // Get session id value on submit
      if (sessionIdInput) {
        console.log("Session ID:", sessionIdInput.value);
      }
    });
  }
});
