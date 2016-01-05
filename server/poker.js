var io = require('socket.io')({
	transports: ['websocket'],
});

io.attach(3000);

roomid = 0;

io.on('connection', function(socket){
	console.log("new connection: " + socket.id);
	socket.emit('msg', {message: 'you are connected to server'});
	
	socket.on('beep', function(){
		socket.emit('boop', {this : 'thisString', that: 'thatString'});
	});

	socket.on('match', function(){
		var rooms = io.sockets.adapter.rooms;
		console.log('iterating all rooms :');
		for (var roomKey in Object.keys(rooms))
		{
			console.log("room with key: " + roomKey);
			if (rooms[roomKey] == undefined)
			{
				continue;
			}
			if (Object.keys(rooms[roomKey]).length == 1)
			{
				socket.join(roomKey);
				io.sockets.in(roomKey).emit('match', {found: true});
				io.sockets.in(roomKey).emit('msg', {message: "match found in the room " + roomKey});
				roomid += 1;
				playGame(rooms[roomKey], roomKey);
				return;
			}
		}
		/*
		for (var room in rooms)
		{
			console.log("inspecting ");
			if (Object.keys(room).length == 1)
			{
				socket.join(room);
				io.sockets.in(room).emit('match', {found: true});
				io.sockets.in(room).emit('msg', {message: "match found in the room " + roomid.toString()});
				roomid += 1;
				playGame(room, roomid-1);
				return;
			}
		}
		*/
		socket.join(roomid.toString());
		socket.emit('match', {found: false});
		socket.emit('msg', {message: "waiting in the room " + roomid.toString()});
	});

	socket.on('disconnect', function(){
		var socketRooms = socket.rooms;
		for (var key in socketRooms)
		{
			socket.leave(key);
			io.sockets.in(key.emit('disconnect'));
			var clients = io.sockets.adapter.rooms.sockets;
			for (var client in clients)
			{
				client.leave(key);
			}
		}
	});
});

function playGame(room, roomKey)
{
	var players = Object.keys(room);
	console.log(players);
	var id1 = players[0];
	var id2 = players[1];

	var game = {};

	game.roomKey = roomKey;

	game.p1 = io.sockets.connected[id1];
	game.p2 = io.sockets.connected[id2];
	var p1 = game.p1;
	var p2 = game.p2;

	p1.emit('msg', {message : "again, you are player 1"});
	p2.emit('msg', {message : "again, you are player 2"});

	game.deck = [];
	setupDeck(game.deck);

	//initial card deal
	game.p1hand = [game.deck.pop(), game.deck.pop()];
	game.p2hand = [game.deck.pop(), game.deck.pop()];
	game.common = []
	p1.emit('deal', {cards: game.p1hand});
	p2.emit('deal', {cards: game.p2hand});

	game.p1sum = 0;
	game.p2sum = 0;
	game.flopReceived = false;
	game.turnReceived = false;
	game.riverReceived = false;
	game.resultReceived = false;

	p1.on('fold', function(){
		p2.emit('fold');
	});
	p2.on('fold', function(){
		p1.emit('fold');
	});
	
	p1.on('bet', function(data){
		var p1bet = data['amount'];
		console.log("player1 raised bet from " + game.p1sum + " to " + p1bet + ".");
		if (p1bet < game.p1sum)
		{
			console.log("error : new bet cannot be lower!");
			//handle negative bet
		}
		p2.emit('bet', {amount: p1bet});
		game.p1sum = p1bet;
	});
	p2.on('bet', function(data){
		var p2bet = data['amount'];
		console.log("player1 raised bet from " + game.p2sum + " to " + p2bet + ".");
		if (p2bet < game.p2sum)
		{
			console.log("error : new bet cannot be lower!");
			//handle negative bet
		}
		p1.emit('bet', {amount: p2bet});
		game.p2sum = p2bet;
	});

	p1.on('flop', function()
	{
		handleFlop(game);
		
	});

	p2.on('flop', function()
	{
		handleFlop(game);
	});

	p1.on('turn', function()
	{
		handleTurn(game);
	});

	p2.on('turn', function()
	{
		handleTurn(game);
	});

	p1.on('river', function()
	{
		handleRiver(game);
	});
	p2.on('turn', function()
	{
		handleRiver(game);
	});

	p1.on('result', function()
	{
		handleResult(game);
	});
	p2.on('result', function()
	{
		handleResult(game);
	});

}

function handleFlop(game)
{
	if (!game.flopReceived)
	{
		game.common.push(game.deck.pop());
		game.common.push(game.deck.pop());
		game.common.push(game.deck.pop());
		io.sockets.in(game.roomKey).emit('flop', { cards: game.common });
		game.flopReceived = true;
	}
}

function handleTurn(game)
{
	if (!game.turnReceived)
	{
		var card = game.deck.pop();
		io.sockets.in(game.roomKey).emit('turn', { cards: [card] });
		game.turnReceived = true;
	}
}

function handleRiver(game)
{
	if (!game.riverReceived)
	{
		var card = game.deck.pop();
		io.sockets.in(game.roomKey).emit('river', { cards: [card] });
		game.riverReceived = true;
	}
}

function handleResult(game)
{
	if (!game.resultReceived)
	{
		game.p1.emit('result', {win : true, cards: game.p2hand});
		game.p2.emit('result', {win : true, cards: game.p1hand});
		game.resultReceived = true;
	}
}

function setupDeck(deck)
{
	for (var i = 1; i < 14; i++)
	{
		deck.push({number: i, shape: "Heart"});
		deck.push({number: i, shape: "Spade"});
		deck.push({number: i, shape: "Club"});
		deck.push({number: i, shape: "Diamond"});		
	}
	shuffle(deck);
}

function shuffle(array)
{
	var m = array.length, t, i;
	while (m)
	{
		i = Math.floor(Math.random() * m--);
		t = array[m];
		array[m] = array[i];
		array[i] = t;
	}
	
}