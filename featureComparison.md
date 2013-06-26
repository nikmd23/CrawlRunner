<!--- Table & Footnote support via Markdown Extra: http://michelf.ca/projects/php-markdown/extra/ --->
| Feature				| CrawlRunner	| [Selenium][1]	| [xUnit][2]	| [qUnit][3]	|
| --------------------- | :-----------: | :-----------: | :-----------: | :-----------: |
| Unit Testing			| 				| 				| X				| X				|
| Integration Testing	| X				| X				| X				| X				|
| Functional Testing	| X				| X				| X				| X				|
| Test HTTP Headers		| X				| 				| 				| 				|
| Test Response Times	| X				| 				| 				| 				|
| Test HTML Output		| X				| X[^1]			| 				| X				|
| Test JavaScript		|				| X				| 				| X				|
| Cross Browser			| X[^2]			| X				| 				| X				|
| Headless Execution	| X				| X[^3]			| X				| X[^4]			|

<!--- Footnotes --->
[^1]: Visible output only. No support for elements with `display:none`, `visibility:hidden` or covered via `z-index`.
[^2]: Support via user-agent switching.
[^3]: Headless support via [PhantomJS](http://phantomjs.org/) driver.
[^4]: Headless support via a runner that supports [PhantomJS](http://phantomjs.org/) like [Chutzpah](http://chutzpah.codeplex.com/).

<!--- Hyperlinks --->
[1]: http://seleniumhq.org/
[2]: http://xunit.codeplex.com/
[3]: http://qunitjs.com/