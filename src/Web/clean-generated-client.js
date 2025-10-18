const fs = require('fs');
const path = require('path');

// Path to the generated client file
const clientFilePath = path.join(__dirname, '../client_web/src/client.ts');

// Read the file
let content = fs.readFileSync(clientFilePath, 'utf8');

// Remove any absolute paths that might have been added
// This regex matches lines that start with / and contain the project path
content = content.replace(/^\/Users\/.*$/gm, '');

// Remove any trailing empty lines
content = content.replace(/\n\n+$/g, '\n');

// Write the cleaned content back
fs.writeFileSync(clientFilePath, content, 'utf8');

console.log('✅ Cleaned generated client.ts file');
