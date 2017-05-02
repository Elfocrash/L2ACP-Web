/*
 * Copyright (C) 2017  Nick Chapsas
 * This program is free software: you can redistribute it and/or modify it under
 * the terms of the GNU General Public License as published by the Free Software
 * Foundation, either version 2 of the License, or (at your option) any later
 * version.
 * 
 * L2ACP is distributed in the hope that it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
 * FOR A PARTICULAR PURPOSE. See the GNU General Public License for more
 * details.
 * 
 * You should have received a copy of the GNU General Public License along with
 * this program. If not, see <http://www.gnu.org/licenses/>.
 */
namespace L2ACP.Models
{
    public class PlayerInfo
    {
        public string Name { get; set; }
        public string Title { get; set; }
        public int Level { get; set; }
        public int Pvp { get; set; }
        public int Pk { get; set; }
        public int Sex { get; set; }
        public int Race { get; set; }
        public string ClanName { get; set; }
        public string AllyName { get; set; }
        public bool Hero { get; set; }
        public bool Nobless { get; set; }
        public long Time { get; set; }
    }
}