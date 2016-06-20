A = {'11,22,33';'44,55,66';'77,88,99'};
B = cell2mat(A);
C = strrep(A,',',' ');
D = cell2mat(C);
E = str2num(D);
F = str2num(B);

obj1 = instrfind('Type', 'serial', 'Port', 'COM4', 'Tag', '');
n = 0;
count = 0;

% Create the serial port object if it does not exist
% otherwise use the object that was found.
if isempty(obj1)
    obj1 = serial('COM4');
else
    fclose(obj1);
    obj1 = obj1(1);
end

% Configure instrument object, obj1.
set(obj1, 'Terminator', {13,13});
% Configure instrument object, obj1.
set(obj1, 'Timeout', 3);
set(obj1, 'BaudRate', 9600);

% set(obj1, 'BytesAvailableFcnMode', 'terminator');

% anonf = @(src,event) (disp(fgetl(src)));
% obj1.BytesAvailableFcn = anonf;

% Connect to instrument object, obj1.
fopen(obj1);

timerVal = tic;
dtime = [];
toctime = [];

timey = timer('Period', .5, 'ExecutionMode', 'fixedSpacing');
timey.TimerFcn = {@plotData};

for i = 0:100
    toctime = [(toc(timerVal)); toctime];
    received = cellstr(fscanf(obj1));
    received = strrep(received,sprintf('\n'),'');
    dtime = [{(received{1}(2:end))}; dtime];
end

a = cellfun(@num2str, dtime,'UniformOutput',0);
b = regexp(dtime,',','split');
c=cellfun(@str2num,dtime,'UniformOutput',0); 
d = cell2mat(c);
