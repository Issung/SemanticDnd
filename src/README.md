https://medium.com/@masreis/text-extraction-and-ocr-with-apache-tika-302464895e5f

## Database
https://dbdiagram.io/d/CustomFields-6832b4bdb9f7446da305fa49

## Development
Start the frontend SPA inside DndTest/App with `npm run dev` or open that folder in VS Code and launch it.
Launch the backend code, it has a proxy to route you to the frontend, so open `https://localhost:7223/app/` in your browser.

## Scraps

What's going on:

* Postgres with vector extension
* Custom built docker image - tika with english tesseract 
* Converting docx files to pdf with libreoffice "soffice.exe" cli before sending to tika so tika can tell us where pages begin & end.

TODO:

* Convert docxs to pdfs to get page by page info.
* Look into improving the indexing and search query https://chatgpt.com/share/67f1057a-7538-8012-a9e7-fbcfb113e7bf
	* Lower case all the text used for the text index & in queries as well.

* ~~Create migration for TikaCache table~~
* ~~Issues saving Vector to database with new nuget types. Check https://github.com/pgvector/pgvector-dotnet/blob/master/tests/Pgvector.CSharp.Tests/EntityFrameworkCoreTests.cs~~
* ~~Document upload page (name & file inputs)~~
* ~~Save document~~
* ~~Document list page~~
* ~~Send document to Tika~~
* ~~Breakup into pages (best-guess for docx) (should docx's be converted to pdfs first before sent to tika? how?)~~
* ~~Generate embeddings for pages and store search chunks~~
* ~~Search page (execute search and show results, jump to page).~~