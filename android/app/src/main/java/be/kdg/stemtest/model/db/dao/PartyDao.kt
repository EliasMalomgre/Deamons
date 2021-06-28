package be.kdg.stemtest.model.db.dao

import androidx.room.*
import be.kdg.stemtest.model.entity.Party
import io.reactivex.Completable
import io.reactivex.Maybe
import io.reactivex.Single

@Dao
abstract class PartyDao {
    @Insert
    abstract fun insert(party: Party)

    @Update
    abstract fun update(party: Party) : Completable

    @Query("SELECT * FROM Party")
    abstract fun getAllParties(): Maybe<List<Party>>

    @Query("SELECT * FROM Party WHERE name=:partij")
    abstract fun getParty(partij:String): Single<Party>

    @Query("SELECT * FROM Party WHERE  chosen=1")
    abstract fun getChosenParty(): Single<Party>

    @Transaction
    open fun insertAllParties(parties:List<Party>){
        for (party in parties) {
            insert(party)
        }
    }
}