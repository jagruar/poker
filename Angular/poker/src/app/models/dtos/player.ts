import { PLayerStatus } from "./player-status";
import { PlayerRoundStatus } from './player-round-status';
import { Card } from './card';

export interface Player {
    id: string;
    name: string;
    seatNumber: number;
    balance: number;
    amountBet: number;
    totalAmountBet: number;
    status: PLayerStatus;
    roundStatus: PlayerRoundStatus;
    cards: Card[];
}