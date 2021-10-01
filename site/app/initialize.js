const md = require('content.html.pug');
const hljs = require('highlightjs');

document.addEventListener('DOMContentLoaded', () => {
  // do your setup here
  console.log('Initialized app');
  document.getElementById('main').innerHTML = md;
  document.querySelectorAll('code').forEach((block) => {
    hljs.highlightBlock(block);
  });
});