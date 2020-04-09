import { BettingRound } from "./betting-round";
import { Player } from './player';
import { Card } from './card';

export interface Game {
    name: string;
    maxPlayers: number;
    myProperty: number;
    smallBlind: number;
    bigBlind: number;
    minimumBetIncrement: number;
    dealerSeatNumber: number;
    smallBlindSeatNumber: number;
    bigBlindSeatNumber: number;
    nextSeatNumber: number;
    bettingRound: BettingRound;
    pot: number;
    players: Player[];
    flop: Card[];
    river: Card;
    turn: Card;
}