/*-----------------------------------------------------------------------------
 * Scoring.cs - Keeps track of the player's score and levels up as required.
 * Copyright (C) 2010 Justin Lloyd
 * http://www.otakunozoku.com/
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 3 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU lesser General Public License
 * along with this library.  If not, see <http://www.gnu.org/licenses/>.
 *
-----------------------------------------------------------------------------*/

using System;

public class Achievements {
    
    public static Achievement[] achievements;
	
	public static Achievement[] getByTag(int tag) {
		return achievements;
	}
	
	public static void add(Achievement a) {
		achievements[0] = a;
	}
	
}

public struct Achievement {
	
	public Action condition;
	
}