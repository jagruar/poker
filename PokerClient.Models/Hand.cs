using System;
using System.Collections.Generic;
using System.Linq;

namespace PokerClient.Models
{
    /// <summary>
    /// 5 cards
    /// </summary>
    public class Hand
    {
        private string _value;
        private readonly IEnumerable<int> _values;
        private readonly bool _isFlush;
        private bool _isStraight;
        private bool _usingLow;

        public string Value
        {
            get
            {
                if (!string.IsNullOrEmpty(_value))
                {
                    return _value;
                }
                else
                {
                    _value = CalculateValue();
                    return _value;
                }
            }
        }


        public Hand(IEnumerable<Card> cards)
        {
            if (cards.Count() != 5)
            {
                throw new Exception();
            }

            _values = cards.Select(x => x.Value).OrderByDescending(x => x);         
            _isFlush = cards.GroupBy(x => x.Suit).Count() == 1;
            CalculateStraight();

        }

        private string CalculateValue()
        {
            // straight flush
            if (_isFlush && _isStraight)
            {
                return 
                    "9"
                    + (_usingLow ? "5" : ToStringValue(_values.First()));
            }

            var valueGroups = _values.GroupBy(x => x).OrderByDescending(x => x.Count());

            // Four of a kind
            if (valueGroups.First().Count() == 4)
            {
                return 
                    "8" 
                    + ToStringValue(valueGroups.First().First())
                    + ToStringValue(valueGroups.Skip(1).First().First());
            }

            // Full House
            if (valueGroups.Count() == 2)
            {
                return
                    "7"
                    + ToStringValue(valueGroups.First().First())
                    + ToStringValue(valueGroups.Skip(1).First().First());
            }

            if (_isFlush)
            {
                return
                    "6"
                    + ToStringValue(_values.First())
                    + ToStringValue(_values.Skip(1).First())
                    + ToStringValue(_values.Skip(2).First())
                    + ToStringValue(_values.Skip(3).First())
                    + ToStringValue(_values.Skip(4).First());
            }

            if (_isStraight)
            {
                return
                    "5"
                    + (_usingLow ? "5" : ToStringValue(_values.First()));
            }

            // Trips
            if (valueGroups.First().Count() == 3)
            {
                return
                    "4"
                    + ToStringValue(valueGroups.First().First())
                    + ToStringValue(valueGroups.Skip(1).First().First())
                    + ToStringValue(valueGroups.Skip(2).First().First());
            }

            // Two Pair
            if (valueGroups.Count() == 3)
            {
                return
                    "3"
                    + ToStringValue(valueGroups.First().First())
                    + ToStringValue(valueGroups.Skip(1).First().First())
                    + ToStringValue(valueGroups.Skip(2).First().First());
            }

            // Pair
            if (valueGroups.Count() == 4)
            {
                return
                    "2"
                    + ToStringValue(valueGroups.First().First())
                    + ToStringValue(valueGroups.Skip(1).First().First())
                    + ToStringValue(valueGroups.Skip(2).First().First())
                    + ToStringValue(valueGroups.Skip(3).First().First());
            }

            // High Card
            return
                "1"
                + ToStringValue(_values.First())
                + ToStringValue(_values.Skip(1).First())
                + ToStringValue(_values.Skip(2).First())
                + ToStringValue(_values.Skip(3).First())
                + ToStringValue(_values.Skip(4).First());
        }

        private void CalculateStraight()
        {
            if (IsConsecutive(_values))
            {
                _isStraight = true;
            }
            else
            {
                var lowValues = _values.Select(x => x == 14 ? 1 : x);
                if (IsConsecutive(lowValues))
                {
                    _isStraight = true;
                    _usingLow = true;
                }
            }
        }

        private bool IsConsecutive(IEnumerable<int> values)
        {
            return !values.Select((i, j) => i - j).Distinct().Skip(1).Any();
        }

        private string ToStringValue(int i)
        {
            return i switch
            {
                14 => "e",
                13 => "d",
                12 => "c",
                11 => "b",
                10 => "a",
                _ => i.ToString(),
            };
        }
    }
}
