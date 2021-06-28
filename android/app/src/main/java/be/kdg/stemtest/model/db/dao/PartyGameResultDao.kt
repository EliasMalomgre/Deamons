package be.kdg.stemtest.model.db.dao

import androidx.room.Dao
import androidx.room.Insert
import androidx.room.Query
import be.kdg.stemtest.model.entity.PartyGameResult
import io.reactivex.Maybe

@Dao
interface PartyGameResultDao {

    @Insert
    fun insert(result: PartyGameResult)

    @Query("SELECT * FROM partygameresult")
    fun getResult(): Maybe<PartyGameResult>
}