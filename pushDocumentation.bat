rmdir doc /S /Q
md doc
cd doc
git clone https://github.com/PapaMufflon/StandUpTimer.git --branch gh-pages
cd StandUpTimer
copy ..\..\build\results\StandUpTimer\Specs\Index.html .\ /Y
git commit -am "Updated documentation"
git push
cd..
cd..