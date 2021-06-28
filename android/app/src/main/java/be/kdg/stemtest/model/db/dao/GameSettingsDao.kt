package be.kdg.stemtest.model.db.dao

import androidx.room.Dao
import androidx.room.Insert
import androidx.room.Query
import be.kdg.stemtest.model.entity.GameSettings
import io.reactivex.Completable
import io.reactivex.Maybe

@Dao
interface GameSettingsDao {


    @Insert
    fun insert(gameSettings: GameSettings) : Completable

    @Query("SELECT * FROM gamesettings")
    fun getSettings() : Maybe<GameSettings>
}