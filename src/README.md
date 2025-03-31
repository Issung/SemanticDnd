https://medium.com/@masreis/text-extraction-and-ocr-with-apache-tika-302464895e5f

What's going on:

* Postgres with vector extension
* Custom built docker image - tika with english tesseract 
* Converting docx files to pdf with libreoffice "soffice.exe" cli before sending to tika so tika can tell us where pages begin & end.

TODO:

* ~~Create migration for TikaCache table~~
* Issues saving Vector to database with new nuget types. Check https://github.com/pgvector/pgvector-dotnet/blob/master/tests/Pgvector.CSharp.Tests/EntityFrameworkCoreTests.cs
* Document upload page (name & file inputs)
* Save document
* Document list page
* Send document to Tika
* Breakup into pages (best-guess for docx) (should docx's be converted to pdfs first before sent to tika? how?)
* Generate embeddings for pages and store search chunks
* Search page (execute search and show results, jump to page).