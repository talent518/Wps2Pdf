<?php

$pathName = isset($_SERVER['argv'][1]) ? $_SERVER['argv'][1] : 'docs';

$socket = socket_create(AF_INET, SOCK_STREAM, SOL_TCP);

socket_connect($socket, '127.0.0.1', 2012) or ErrMsg($socket, '连接错误');

$docsPath = dirname(__FILE__) . '/' . $pathName . '/';

($dh = @opendir($docsPath)) or ErrMsg($socket, '打开./' . $pathName . '/目录失败');

$commands = array();
while (($file = readdir($dh)) !== false) {
	$filePath = realpath($docsPath.$file);

	if(is_dir($filePath)) {
		continue;
	}

	if(!strcasecmp(pathinfo($filePath, PATHINFO_EXTENSION), 'pdf')) {
		@unlink($filePath);
		continue;
	}

	$commands[] = 'Convert ' . $filePath . PHP_EOL;
}

$count = count($commands);
foreach($commands as $n=>$command) {
	echo $n + 1, '/', $count, PHP_EOL, $command;

	@socket_write($socket, iconv('GBK', 'UTF-8', $command)) !== false or ErrMsg($socket, '发送转换请求失败');

	do {
		$reads = array($socket);
		$writes = $excepts = array();
		while(($ret = @socket_select($reads, $writes, $excepts, 1)) === 0) {
			@socket_write($socket, 'Ping ...' . PHP_EOL);
		}
		if($ret === false) {
			ErrMsg($socket, '发送转换请求失败');
		}

		$buffer = trim(@socket_read($socket, 2048));
		echo $buffer, PHP_EOL;
	} while($buffer === '...' || $buffer === 'WaitConvert');

	echo PHP_EOL;
}

@socket_close($socket);

function ErrMsg($socket, $message) {
	$errno = socket_last_error($socket);
	if($errno === 0) {
		return;
	}

	echo $message, '(', $errno, ')：', socket_strerror($errno), PHP_EOL;

	@socket_close($socket);

	exit;
}
