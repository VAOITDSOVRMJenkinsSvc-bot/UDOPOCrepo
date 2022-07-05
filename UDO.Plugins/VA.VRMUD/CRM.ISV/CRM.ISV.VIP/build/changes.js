var source, destination, scmPath, action, batch, scm;

if (process.argv.length >= 1 && process.argv.length < 6) {

    source = process.argv[2];
    destination = process.argv[3];
    scmPath = process.argv[4];

    console.log('process started...');

    batch = require('child_process');
    scm = batch.spawn(scmPath + '\\' + 'lscm.bat', ['status', '-w']);

    scm.stdout.on('data', function(data) {
        var outArray = data.toString().split('\r\n');

        for (var i = 0; i < outArray.length; i++) {
            var file = getFileName(trim(outArray[i]))
            if (!file) continue;

            if (file.action.toLowerCase === 'd') {
                // TODO delete process
            } else {
                action = batch.spawn('xcopy', [source + '\\' + file.section + '\\' + file.name, destination + '\\' + file.section + '\\' + file.name, '/Y', '/R']);
            }

            action.stdout.on('data', function(data) {
                console.log('xcopy : ' + data);
            });

            action.stderr.on('data', function(data) {
                console.log('xcopy error: ' + data);
            });
        }
    });

    scm.stderr.on('data', function(data) {
        console.log('scm error: ' + data);
    });

    scm.on('close', function(code) {
        console.log('scm process exited with code ' + code);
    });
    
}

function getFileName(line) {
    var lineParts, action, section, file, linePathAll, sectionPath, sectionPathParts;
    
	if (line.match(/\.js$/) && line.match(/\/app\//)) {
	    lineParts = line.split(' ');
	    
        if (lineParts.length > 0) {
            action = lineParts[0];
            linePathAll = line.substring(line.indexOf(action) + action.length);

            sectionPath = linePathAll.substring(linePathAll.indexOf('app') + 4);
            
            sectionPathParts = sectionPath.split('/');

            file = sectionPathParts[sectionPathParts.length - 1];
            section = sectionPath.substring(0, sectionPath.indexOf(file) - 1)

            if (!file) return null;
            
            return {
			    name: file,
			    section: section.replace(/\//g, '\\'), 
			    action: action.replace(/-/g,''),
			    toString: function() {
			        return this.action + ' - ' + this.section + '\\' + this.name;
			    }
			}
        }
	}
	
	return null;
}

function trim(str) {
  return str.replace(/^\s+|\s+$/g, '');
}